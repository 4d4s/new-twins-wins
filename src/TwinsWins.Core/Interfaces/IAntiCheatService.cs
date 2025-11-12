using TwinsWins.Core.DTOs;

namespace TwinsWins.Core.Interfaces;

public interface IAntiCheatService
{
    Task<bool> ValidateMoveTimingAsync(Guid gameId, Guid userId, GameMoveDto move);
    Task<bool> DetectBotPatternAsync(Guid gameId, Guid userId);
    Task<bool> ValidateHeartbeatAsync(Guid gameId, Guid userId);
    Task<bool> ValidateGameStateAsync(Guid gameId, string clientStateHash);
}
