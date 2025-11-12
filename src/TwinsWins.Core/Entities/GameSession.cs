namespace TwinsWins.Core.Entities;

public class GameSession : BaseEntity
{
    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string ConnectionId { get; set; } = string.Empty;
    public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public DateTime? DisconnectedAt { get; set; }
}
