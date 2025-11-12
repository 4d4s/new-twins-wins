using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;

namespace TwinsWins.BlazorClient.Services;

public class AuthService : IAuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IWalletService _walletService;

    public AuthService(ILocalStorageService localStorage, IWalletService walletService)
    {
        _localStorage = localStorage;
        _walletService = walletService;
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>("auth_token");
    }

    public async Task<bool> LoginAsync(string walletAddress, string signature)
    {
        // TODO: Implement actual authentication
        // 1. Send wallet address and signature to API
        // 2. Receive JWT token
        // 3. Store token
        
        // Stub implementation
        var token = "stub_jwt_token_" + Guid.NewGuid().ToString();
        await _localStorage.SetItemAsync("auth_token", token);
        return true;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("auth_token");
        await _walletService.DisconnectWalletAsync();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}

public class GameHubService : IGameHubService
{
    private HubConnection? _hubConnection;
    
    public event Action<object>? OnMoveCompleted;
    public event Action<object>? OnGameCompleted;
    public event Action<string>? OnUserJoined;
    public event Action<string>? OnUserLeft;

    public async Task StartAsync(string token)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:5000/hubs/game", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<object>("MoveCompleted", (result) =>
        {
            OnMoveCompleted?.Invoke(result);
        });

        _hubConnection.On<object>("GameCompleted", (result) =>
        {
            OnGameCompleted?.Invoke(result);
        });

        _hubConnection.On<string>("UserJoined", (userId) =>
        {
            OnUserJoined?.Invoke(userId);
        });

        _hubConnection.On<string>("UserLeft", (userId) =>
        {
            OnUserLeft?.Invoke(userId);
        });

        await _hubConnection.StartAsync();
    }

    public async Task JoinGameAsync(string gameId)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("JoinGame", gameId);
        }
    }

    public async Task LeaveGameAsync(string gameId)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("LeaveGame", gameId);
        }
    }

    public async Task SendHeartbeatAsync(string gameId)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("SendHeartbeat", gameId);
        }
    }
}
