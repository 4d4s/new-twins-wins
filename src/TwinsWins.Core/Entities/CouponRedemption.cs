using TwinsWins.Core.Enums;

namespace TwinsWins.Core.Entities;

public class CouponRedemption : BaseEntity
{
    public Guid CouponId { get; set; }
    public Coupon Coupon { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string UserWalletAddress { get; set; } = string.Empty;
    public decimal RedeemedAmount { get; set; }
    public string? BlockchainTxHash { get; set; }
    public BlockchainTransactionStatus TransactionStatus { get; set; } = BlockchainTransactionStatus.Pending;
    public DateTime RedeemedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
}