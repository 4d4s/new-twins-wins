using TwinsWins.Core.DTOs;

namespace TwinsWins.Core.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Generate a nonce for wallet signature
    /// </summary>
    Task<string> GenerateNonceAsync(string walletAddress);

    /// <summary>
    /// Verify wallet signature and authenticate user
    /// </summary>
    Task<AuthResponseDto> AuthenticateAsync(string walletAddress, string signature, string nonce, string? referralCode = null);

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Validate JWT token
    /// </summary>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    Task RevokeTokenAsync(string refreshToken);
}