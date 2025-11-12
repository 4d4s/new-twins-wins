using TwinsWins.Core.Enums;

namespace TwinsWins.Core.Entities;

public class Transaction : BaseEntity
{
    public string WalletAddress { get; set; } = string.Empty;
    public Guid? GameId { get; set; }
    public Game? Game { get; set; }
    public string? TransactionHash { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TON";
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    public string? FailureReason { get; set; }
    public int RetryCount { get; set; } = 0;
}
