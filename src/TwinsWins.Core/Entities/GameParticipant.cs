using TwinsWins.Core.Enums;

namespace TwinsWins.Core.Entities;

public class GameParticipant : BaseEntity
{
    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public ParticipantRole Role { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public int? Score { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool? IsWinner { get; set; }
    public decimal? PayoutAmount { get; set; }
}
