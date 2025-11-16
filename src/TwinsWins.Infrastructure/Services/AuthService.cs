using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TwinsWins.Core.DTOs;
using TwinsWins.Core.Entities;
using TwinsWins.Core.Interfaces;
using TwinsWins.Infrastructure.Data;

namespace TwinsWins.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    private const string NONCE_PREFIX = "auth:nonce:";
    private const int NONCE_EXPIRATION_SECONDS = 300; // 5 minutes

    public AuthService(
        ApplicationDbContext context,
        IDistributedCache cache,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _context = context;
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GenerateNonceAsync(string walletAddress)
    {
        // Generate a cryptographically secure random nonce
        var nonce = GenerateSecureNonce();

        // Store nonce in Redis with expiration
        var cacheKey = $"{NONCE_PREFIX}{walletAddress}";
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(NONCE_EXPIRATION_SECONDS)
        };

        await _cache.SetStringAsync(cacheKey, nonce, cacheOptions);

        _logger.LogInformation("Generated nonce for wallet {WalletAddress}", walletAddress);

        return nonce;
    }

    public async Task<AuthResponseDto> AuthenticateAsync(
        string walletAddress,
        string signature,
        string nonce,
        string? referralCode = null)
    {
        // 1. Validate nonce
        var cacheKey = $"{NONCE_PREFIX}{walletAddress}";
        var storedNonce = await _cache.GetStringAsync(cacheKey);

        if (string.IsNullOrEmpty(storedNonce) || storedNonce != nonce)
        {
            _logger.LogWarning("Invalid or expired nonce for wallet {WalletAddress}", walletAddress);
            throw new UnauthorizedAccessException("Invalid or expired nonce");
        }

        // 2. Verify TON wallet signature
        if (!VerifyTonSignature(walletAddress, nonce, signature))
        {
            _logger.LogWarning("Invalid signature for wallet {WalletAddress}", walletAddress);
            throw new UnauthorizedAccessException("Invalid signature");
        }

        // 3. Remove used nonce
        await _cache.RemoveAsync(cacheKey);

        // 4. Get or create user
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.WalletAddress == walletAddress);

        if (user == null)
        {
            user = await CreateNewUserAsync(walletAddress, referralCode);
        }
        else
        {
            // Update last active
            user.LastActiveAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // Check if user is blocked
        if (user.IsBlocked)
        {
            _logger.LogWarning("Blocked user attempted to authenticate: {UserId}", user.Id);
            throw new UnauthorizedAccessException("User account is blocked");
        }

        // 5. Generate JWT tokens
        var (accessToken, refreshToken) = await GenerateTokensAsync(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = GetJwtExpirationSeconds(),
            User = MapToUserDto(user)
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _context.Set<RefreshToken>()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        if (storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token is expired or revoked");
        }

        if (storedToken.User.IsBlocked)
        {
            throw new UnauthorizedAccessException("User account is blocked");
        }

        // Revoke old token
        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;

        // Generate new tokens
        var (newAccessToken, newRefreshToken) = await GenerateTokensAsync(storedToken.User);

        storedToken.ReplacedByToken = newRefreshToken;
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = GetJwtExpirationSeconds(),
            User = MapToUserDto(storedToken.User)
        };
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(GetJwtSecret());

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = GetJwtIssuer(),
                ValidateAudience = true,
                ValidAudience = GetJwtAudience(),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var token = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token != null)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    #region Private Methods

    private async Task<User> CreateNewUserAsync(string walletAddress, string? referralCode)
    {
        var user = new User
        {
            WalletAddress = walletAddress,
            AffiliateCode = GenerateAffiliateCode(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LastActiveAt = DateTime.UtcNow
        };

        // Handle referral
        if (!string.IsNullOrEmpty(referralCode))
        {
            var referrer = await _context.Users
                .FirstOrDefaultAsync(u => u.AffiliateCode == referralCode);

            if (referrer != null)
            {
                user.ReferredByUserId = referrer.Id;

                // Create affiliate link
                var affiliateLink = new AffiliateLink
                {
                    ReferrerUserId = referrer.Id,
                    ReferredUserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Set<AffiliateLink>().Add(affiliateLink);
            }
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created new user {UserId} for wallet {WalletAddress}",
            user.Id, walletAddress);

        return user;
    }

    private async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user)
    {
        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        // Store refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays()),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Set<RefreshToken>().Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return (accessToken, refreshToken);
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(GetJwtSecret());

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.WalletAddress),
            new Claim("wallet_address", user.WalletAddress),
            new Claim("affiliate_code", user.AffiliateCode)
        };

        if (!string.IsNullOrEmpty(user.Username))
        {
            claims.Add(new Claim(ClaimTypes.GivenName, user.Username));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(GetJwtExpirationMinutes()),
            Issuer = GetJwtIssuer(),
            Audience = GetJwtAudience(),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private string GenerateSecureNonce()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private string GenerateAffiliateCode()
    {
        // Generate a unique 6-character alphanumeric code
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        string code;

        do
        {
            code = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        while (_context.Users.Any(u => u.AffiliateCode == code));

        return code;
    }

    private bool VerifyTonSignature(string walletAddress, string nonce, string signature)
    {
        // TODO: Implement actual TON signature verification
        // This requires TonSdk.NET or similar library
        // For now, we'll use a placeholder implementation

        // In production, this should:
        // 1. Reconstruct the message that was signed (e.g., "Sign this message to authenticate: {nonce}")
        // 2. Verify the signature using TON's Ed25519 signature verification
        // 3. Check that the public key matches the wallet address

        _logger.LogWarning("Using placeholder signature verification - implement actual TON verification!");

        // Placeholder: In development, accept any non-empty signature
        // REMOVE THIS IN PRODUCTION!
        return !string.IsNullOrEmpty(signature) && signature.Length > 10;
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            WalletAddress = user.WalletAddress,
            Username = user.Username,
            AffiliateCode = user.AffiliateCode,
            SkillRating = user.SkillRating,
            TotalGamesPlayed = user.TotalGamesPlayed,
            TotalWins = user.TotalWins,
            IsBlocked = user.IsBlocked,
            CreatedAt = user.CreatedAt
        };
    }

    private string GetJwtSecret() =>
        _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");

    private string GetJwtIssuer() =>
        _configuration["Jwt:Issuer"] ?? "TwinsWins";

    private string GetJwtAudience() =>
        _configuration["Jwt:Audience"] ?? "TwinsWinsClient";

    private int GetJwtExpirationMinutes() =>
        int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "15");

    private int GetJwtExpirationSeconds() =>
        GetJwtExpirationMinutes() * 60;

    private int GetRefreshTokenExpirationDays() =>
        int.Parse(_configuration["Jwt:RefreshExpirationDays"] ?? "7");

    #endregion
}