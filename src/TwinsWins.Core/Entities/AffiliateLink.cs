namespace TwinsWins.Core.Entities;

public class AffiliateLink : BaseEntity
{
    public Guid ReferrerUserId { get; set; }
    public User Referrer { get; set; } = null!;
    public Guid ReferredUserId { get; set; }
    public User Referred { get; set; } = null!;
    public decimal TotalEarnings { get; set; } = 0;
    public int TotalReferrals { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<AffiliatePayout> Payouts { get; set; } = new List<AffiliatePayout>();
}
