namespace TwinsWins.Core.Entities;

public class User : BaseEntity
{
    public string WalletAddress { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string AffiliateCode { get; set; } = string.Empty;
    public Guid? ReferredByUserId { get; set; }
    public User? ReferredBy { get; set; }
    public int SkillRating { get; set; } = 1000;
    public int TotalGamesPlayed { get; set; } = 0;
    public int TotalWins { get; set; } = 0;
    public DateTime? LastActiveAt { get; set; }
    public bool IsBlocked { get; set; } = false;
    
    // Navigation properties
    public ICollection<GameParticipant> GameParticipants { get; set; } = new List<GameParticipant>();
    public ICollection<AffiliateLink> ReferralsGiven { get; set; } = new List<AffiliateLink>();
    public AffiliateLink? ReferralReceived { get; set; }
}
