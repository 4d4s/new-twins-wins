using TwinsWins.Core.Enums;

namespace TwinsWins.Core.Entities;

public class Game : BaseEntity
{
    public GameType GameType { get; set; }
    public decimal? StakeAmount { get; set; }
    public Guid? ImageSetId { get; set; }
    public ImageSet? ImageSet { get; set; }
    public string LayoutHash { get; set; } = string.Empty;
    public GameStatus Status { get; set; } = GameStatus.Created;
    public string? SmartContractAddress { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? TimeoutAt { get; set; }
    public int Version { get; set; } = 1; // For optimistic locking
    
    // Navigation properties
    public ICollection<GameParticipant> Participants { get; set; } = new List<GameParticipant>();
    public ICollection<GameMove> Moves { get; set; } = new List<GameMove>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
