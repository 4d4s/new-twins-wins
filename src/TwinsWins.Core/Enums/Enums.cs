namespace TwinsWins.Core.Enums;

public enum GameType
{
    Free,
    Paid
}

public enum GameStatus
{
    Created,
    Waiting,
    Active,
    Completed,
    Settling,
    Settled,
    Cancelled
}

public enum ParticipantRole
{
    Creator,
    Joiner
}

public enum TransactionType
{
    Stake,
    Payout,
    Refund,
    Commission,
    Affiliate,
    Coupon
}

public enum TransactionStatus
{
    Pending,
    Confirmed,
    Failed,
    Cancelled
}

public enum CouponType
{
    FixedAmount,
    Percentage,
    FreeGames
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public enum AuditLevel
{
    INFO,
    WARN,
    ERROR,
    CRITICAL
}
