namespace TwinsWins.Core.DTOs;

public class LeaderboardPlayerDto
{
    public string Username { get; set; } = string.Empty;
    public string WalletAddress { get; set; } = string.Empty;
    public int Wins { get; set; }
    public decimal WinRate { get; set; }
    public decimal Earnings { get; set; }
    public int Rating { get; set; }
}