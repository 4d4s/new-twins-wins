using TwinsWins.Core.Enums;

namespace TwinsWins.Core.Entities;

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public CouponType Type { get; set; }
    public decimal Value { get; set; } // Fixed TON amount
    public int MaxUsagePerUser { get; set; } = 1;
    public int? TotalUsageLimit { get; set; }
    public int CurrentUsageCount { get; set; } = 0;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? CreatedBy { get; set; }
    public Guid? CampaignId { get; set; }
    public string? Description { get; set; }

    public ICollection<CouponRedemption> Redemptions { get; set; } = new List<CouponRedemption>();
}
