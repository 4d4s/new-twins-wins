using TwinsWins.Core.Enums;

namespace TwinsWins.Core.Entities;

public class BlockchainTransaction : BaseEntity
{
    public string? TransactionHash { get; set; }
    public BlockchainTransactionType Type { get; set; }
    public string FromWallet { get; set; } = string.Empty;
    public string ToWallet { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public BlockchainTransactionStatus Status { get; set; } = BlockchainTransactionStatus.Pending;
    public Guid? GameId { get; set; }
    public Game? Game { get; set; }
    public Guid? CouponId { get; set; }
    public Coupon? Coupon { get; set; }
    public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    public string? FailureReason { get; set; }
    public string? SmartContractAddress { get; set; }
    public long? BlockNumber { get; set; }
}
