using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwinsWins.Core.DTOs;
using TwinsWins.Core.Interfaces;

namespace TwinsWins.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Generate a nonce for wallet signature
    /// </summary>
    /// <remarks>
    /// Step 1: Client calls this endpoint with their wallet address to get a nonce.
    /// The nonce expires in 5 minutes.
    /// </remarks>
    [HttpPost("nonce")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenerateNonceResponse), 200)]
    public async Task<IActionResult> GenerateNonce([FromBody] GenerateNonceRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.WalletAddress))
            {
                return BadRequest(new { error = "Wallet address is required" });
            }

            var nonce = await _authService.GenerateNonceAsync(request.WalletAddress);

            return Ok(new GenerateNonceResponse
            {
                Nonce = nonce,
                ExpiresInSeconds = 300
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating nonce for wallet {WalletAddress}", request.WalletAddress);
            return StatusCode(500, new { error = "Failed to generate nonce" });
        }
    }

    /// <summary>
    /// Authenticate with wallet signature
    /// </summary>
    /// <remarks>
    /// Step 2: Client signs the nonce with their wallet and sends the signature.
    /// Returns JWT access token and refresh token.
    /// </remarks>
    [HttpPost("authenticate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.WalletAddress))
            {
                return BadRequest(new { error = "Wallet address is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Signature))
            {
                return BadRequest(new { error = "Signature is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Nonce))
            {
                return BadRequest(new { error = "Nonce is required" });
            }

            var response = await _authService.AuthenticateAsync(
                request.WalletAddress,
                request.Signature,
                request.Nonce,
                request.ReferralCode);

            _logger.LogInformation("User authenticated successfully: {UserId}", response.User.Id);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Authentication failed for wallet {WalletAddress}", request.WalletAddress);
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication for wallet {WalletAddress}", request.WalletAddress);
            return StatusCode(500, new { error = "Authentication failed" });
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest(new { error = "Refresh token is required" });
            }

            var response = await _authService.RefreshTokenAsync(request.RefreshToken);

            _logger.LogInformation("Token refreshed successfully for user {UserId}", response.User.Id);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Token refresh failed");
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, new { error = "Token refresh failed" });
        }
    }

    /// <summary>
    /// Revoke refresh token (logout)
    /// </summary>
    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest(new { error = "Refresh token is required" });
            }

            await _authService.RevokeTokenAsync(request.RefreshToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token");
            return StatusCode(500, new { error = "Token revocation failed" });
        }
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), 200)]
    public IActionResult GetCurrentUser()
    {
        // User information is already in the JWT claims
        // For full user data, you might want to fetch from database
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var walletAddress = User.FindFirst("wallet_address")?.Value;
        var affiliateCode = User.FindFirst("affiliate_code")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Return basic info from claims
        // In a real app, you might want to fetch full user data from the database
        return Ok(new
        {
            id = userId,
            walletAddress = walletAddress,
            affiliateCode = affiliateCode
        });
    }
}