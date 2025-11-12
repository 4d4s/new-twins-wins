using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TwinsWins.API.Hubs;

[Authorize]
public class GameHub : Hub
{
    private readonly ILogger<GameHub> _logger;

    public GameHub(ILogger<GameHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        _logger.LogInformation("User {UserId} joined game group {GameId}", 
            Context.UserIdentifier, gameId);
        
        await Clients.Group(gameId).SendAsync("UserJoined", Context.UserIdentifier);
    }

    public async Task LeaveGame(string gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        _logger.LogInformation("User {UserId} left game group {GameId}", 
            Context.UserIdentifier, gameId);
        
        await Clients.Group(gameId).SendAsync("UserLeft", Context.UserIdentifier);
    }

    public async Task SendHeartbeat(string gameId)
    {
        _logger.LogDebug("Heartbeat received from {UserId} for game {GameId}", 
            Context.UserIdentifier, gameId);
        
        // Update last heartbeat timestamp
        await Clients.Caller.SendAsync("HeartbeatAck");
    }

    public async Task NotifyMove(string gameId, object moveResult)
    {
        _logger.LogInformation("Move notification for game {GameId}", gameId);
        await Clients.Group(gameId).SendAsync("MoveCompleted", moveResult);
    }

    public async Task NotifyGameComplete(string gameId, object gameResult)
    {
        _logger.LogInformation("Game {GameId} completed", gameId);
        await Clients.Group(gameId).SendAsync("GameCompleted", gameResult);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client {ConnectionId} connected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogError(exception, "Client {ConnectionId} disconnected with error", 
                Context.ConnectionId);
        }
        else
        {
            _logger.LogInformation("Client {ConnectionId} disconnected", Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}
