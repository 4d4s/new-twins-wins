namespace TwinsWins.Core.DTOs;

public class GenerateNonceRequest
{
    public string WalletAddress { get; set; } = string.Empty;
}

public class GenerateNonceResponse
{
    public string Nonce { get; set; } = string.Empty;
    public int ExpiresInSeconds { get; set; } = 300; // 5 minutes
}

public class AuthenticateRequest
{
    public string WalletAddress { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    public string Nonce { get; set; } = string.Empty;
    public string? ReferralCode { get; set; }
}

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; } // In seconds
    public UserDto User { get; set; } = null!;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string WalletAddress { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string AffiliateCode { get; set; } = string.Empty;
    public int SkillRating { get; set; }
    public int TotalGamesPlayed { get; set; }
    public int TotalWins { get; set; }
    public decimal WinRate => TotalGamesPlayed > 0 ? (decimal)TotalWins / TotalGamesPlayed * 100 : 0;
    public bool IsBlocked { get; set; }
    public DateTime CreatedAt { get; set; }
}