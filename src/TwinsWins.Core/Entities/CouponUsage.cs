namespace TwinsWins.Core.Entities;

public class CouponUsage : BaseEntity
{
    public Guid CouponId { get; set; }
    public Coupon Coupon { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid? GameId { get; set; }
    public Game? Game { get; set; }
    public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    public decimal DiscountAmount { get; set; }
}
