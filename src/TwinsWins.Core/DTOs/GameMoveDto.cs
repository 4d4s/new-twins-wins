namespace TwinsWins.Core.DTOs;

public class GameMoveDto
{
    public int Card1Id { get; set; }
    public int Card2Id { get; set; }
    public long ClientTimestampMs { get; set; }
}

public class GameResultDto
{
    public Guid GameId { get; set; }
    public Guid UserId { get; set; }
    public int Score { get; set; }
    public bool IsCorrect { get; set; }
    public int PointsAwarded { get; set; }
    public int RemainingPairs { get; set; }
    public bool IsGameComplete { get; set; }
    public bool? IsWinner { get; set; }
    public decimal? PayoutAmount { get; set; }
    public decimal PrizeAmount => PayoutAmount ?? 0;
}

public class CreateFreeGameRequest
{
    public Guid ImageSetId { get; set; }
}

public class CreatePaidGameRequest
{
    public decimal StakeAmount { get; set; }
    public Guid ImageSetId { get; set; }
}

public class JoinGameRequest
{
    public Guid GameId { get; set; }
}
