using TwinsWins.Core.DTOs;
using TwinsWins.Core.Entities;

namespace TwinsWins.Core.Interfaces;

public interface IGameService
{
    Task<GameDto> CreateFreeGameAsync(Guid userId, Guid imageSetId);
    Task<GameDto> CreatePaidGameAsync(Guid userId, decimal stakeAmount, Guid imageSetId);
    Task<GameDto> JoinGameAsync(Guid gameId, Guid userId);
    Task<GameResultDto> SubmitMoveAsync(Guid gameId, Guid userId, GameMoveDto move);
    Task<GameDto> GetGameAsync(Guid gameId);
    Task<IEnumerable<GameDto>> GetActiveLobbiesAsync(int skip, int take);
    Task<GameResultDto> CompleteGameAsync(Guid gameId, Guid userId);
    Task CancelExpiredGamesAsync();
}
