namespace TwinsWins.Core.Entities;

public class AffiliatePayout : BaseEntity
{
    public Guid LinkId { get; set; }
    public Guid GameId { get; set; }
    public string ReferrerWalletAddress { get; set; }  // ? Added
    public decimal Amount { get; set; }
    public string? BlockchainTxHash { get; set; }  // ? Changed from TransactionId
    public DateTime PaidAt { get; set; }
}
