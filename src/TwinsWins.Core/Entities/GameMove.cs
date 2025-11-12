namespace TwinsWins.Core.Entities;

public class GameMove : BaseEntity
{
    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public int MoveNumber { get; set; }
    public int Card1Id { get; set; }
    public int Card2Id { get; set; }
    public bool IsCorrect { get; set; }
    public int PointsAwarded { get; set; }
    public long TimestampMs { get; set; } // Milliseconds since game start
    public DateTime ServerValidatedAt { get; set; } = DateTime.UtcNow;
}
