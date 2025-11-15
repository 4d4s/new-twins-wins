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

/// <summary>
/// Types of blockchain transactions (for BlockchainTransactions table)
/// </summary>
public enum BlockchainTransactionType
{
    GameStake,          // Player stakes TON for game
    GamePayout,         // Winner receives payout
    PlatformFee,        // Platform fee collection
    AffiliateFee,       // Affiliate commission payout
    CouponRedemption,   // Coupon redeemed - TON sent to player
    Refund              // Game cancelled - refund to player
}

/// <summary>
/// Status of blockchain transactions
/// </summary>
public enum BlockchainTransactionStatus
{
    Pending,       // Transaction initiated, waiting for blockchain confirmation
    Confirmed,     // Transaction confirmed on blockchain
    Failed         // Transaction failed
}

public enum CouponType
{
    FixedAmount,    // Fixed TON amount (e.g., 10 TON)
    Percentage      // Percentage bonus (not used in simplified schema, but kept for future)
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
