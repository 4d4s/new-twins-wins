using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwinsWins.Core.DTOs;
using TwinsWins.Core.Interfaces;
using System.Security.Claims;

namespace TwinsWins.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly ILogger<GamesController> _logger;

    public GamesController(IGameService gameService, ILogger<GamesController> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }

    /// <summary>
    /// Create a new free game
    /// </summary>
    [HttpPost("free")]
    public async Task<ActionResult<GameDto>> CreateFreeGame([FromBody] CreateFreeGameRequest request)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation("User {UserId} creating free game with ImageSet {ImageSetId}", 
                userId, request.ImageSetId);

            var game = await _gameService.CreateFreeGameAsync(userId, request.ImageSetId);
            return Ok(game);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating free game");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new paid game lobby
    /// </summary>
    [HttpPost("paid")]
    public async Task<ActionResult<GameDto>> CreatePaidGame([FromBody] CreatePaidGameRequest request)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation("User {UserId} creating paid game with stake {Stake}", 
                userId, request.StakeAmount);

            var game = await _gameService.CreatePaidGameAsync(
                userId, 
                request.StakeAmount, 
                request.ImageSetId);

            return Ok(game);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating paid game");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Join an existing game lobby
    /// </summary>
    [HttpPost("{gameId}/join")]
    public async Task<ActionResult<GameDto>> JoinGame(Guid gameId)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation("User {UserId} joining game {GameId}", userId, gameId);

            var game = await _gameService.JoinGameAsync(gameId, userId);
            return Ok(game);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining game {GameId}", gameId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Submit a move in an active game
    /// </summary>
    [HttpPost("{gameId}/moves")]
    public async Task<ActionResult<GameResultDto>> SubmitMove(Guid gameId, [FromBody] GameMoveDto move)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation("User {UserId} submitting move in game {GameId}", userId, gameId);

            var result = await _gameService.SubmitMoveAsync(gameId, userId, move);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting move for game {GameId}", gameId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Complete a game and get final results
    /// </summary>
    [HttpPost("{gameId}/complete")]
    public async Task<ActionResult<GameResultDto>> CompleteGame(Guid gameId)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation("User {UserId} completing game {GameId}", userId, gameId);

            var result = await _gameService.CompleteGameAsync(gameId, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing game {GameId}", gameId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get game details
    /// </summary>
    [HttpGet("{gameId}")]
    public async Task<ActionResult<GameDto>> GetGame(Guid gameId)
    {
        try
        {
            var game = await _gameService.GetGameAsync(gameId);
            return Ok(game);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting game {GameId}", gameId);
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get active game lobbies
    /// </summary>
    [HttpGet("lobbies")]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetActiveLobbies(
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 20)
    {
        try
        {
            var lobbies = await _gameService.GetActiveLobbiesAsync(skip, take);
            return Ok(lobbies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active lobbies");
            return BadRequest(new { error = ex.Message });
        }
    }
}
