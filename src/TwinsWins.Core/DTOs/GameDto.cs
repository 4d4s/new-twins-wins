using TwinsWins.Core.Enums;

namespace TwinsWins.Core.DTOs;

public class GameDto
{
    public Guid Id { get; set; }
    public GameType GameType { get; set; }
    public decimal? StakeAmount { get; set; }
    public GameStatus Status { get; set; }
    public string? SmartContractAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? TimeoutAt { get; set; }
    public List<GameParticipantDto> Participants { get; set; } = new();
    public ImageSetDto? ImageSet { get; set; }
    public List<CardDto> Cards { get; set; } = new();
}

public class GameParticipantDto
{
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public string WalletAddress { get; set; } = string.Empty;
    public ParticipantRole Role { get; set; }
    public int? Score { get; set; }
    public bool? IsWinner { get; set; }
}

public class CardDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int PairId { get; set; }
}

public class ImageSetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
}
