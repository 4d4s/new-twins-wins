using TwinsWins.Core.DTOs;

namespace TwinsWins.BlazorClient.Services;

public interface IGameApiService
{
    Task<GameDto> CreateFreeGameAsync(Guid imageSetId);
    Task<GameDto> CreatePaidGameAsync(decimal stakeAmount, Guid imageSetId);
    Task<GameDto> JoinGameAsync(Guid gameId);
    Task<GameResultDto> SubmitMoveAsync(Guid gameId, GameMoveDto move);
    Task<GameResultDto> CompleteGameAsync(Guid gameId);
    Task<GameDto> GetGameAsync(Guid gameId);
    Task<IEnumerable<GameDto>> GetActiveLobbiesAsync(int skip = 0, int take = 20);
}

public interface IAuthService
{
    Task<string?> GetTokenAsync();
    Task<bool> LoginAsync(string walletAddress, string signature);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
}

public interface IWalletService
{
    Task<bool> ConnectWalletAsync();
    Task DisconnectWalletAsync();
    Task<string?> GetConnectedWalletAsync();
    Task<string> SignMessageAsync(string message);
    event Action<string?>? OnWalletChanged;
}

public interface IGameHubService
{
    Task StartAsync(string token);
    Task JoinGameAsync(string gameId);
    Task LeaveGameAsync(string gameId);
    Task SendHeartbeatAsync(string gameId);
    event Action<object>? OnMoveCompleted;
    event Action<object>? OnGameCompleted;
    event Action<string>? OnUserJoined;
    event Action<string>? OnUserLeft;
}
