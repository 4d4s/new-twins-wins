namespace TwinsWins.Core.Entities;

public class AffiliatePayout : BaseEntity
{
    public Guid LinkId { get; set; }
    public AffiliateLink Link { get; set; } = null!;
    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;
    public decimal Amount { get; set; }
    public Guid? TransactionId { get; set; }
    public Transaction? Transaction { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}
