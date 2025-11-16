using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwinsWins.Core.DTOs;
using TwinsWins.Core.Interfaces;
using System.Security.Claims;

namespace TwinsWins.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    /// Get active game lobbies (PUBLIC - No authentication required)
    /// </summary>
    /// <remarks>
    /// Anyone can view active lobbies without connecting wallet.
    /// This allows users to browse games before deciding to play.
    /// </remarks>
    [HttpGet("lobbies")]
    [AllowAnonymous]  // PUBLIC ENDPOINT
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

    /// <summary>
    /// Get game details (PUBLIC - No authentication required)
    /// </summary>
    /// <remarks>
    /// Anyone can view game details without authentication.
    /// </remarks>
    [HttpGet("{gameId}")]
    [AllowAnonymous]  // PUBLIC ENDPOINT
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
    /// Create a new free game (REQUIRES AUTHENTICATION)
    /// </summary>
    /// <remarks>
    /// User must be authenticated to create a game.
    /// Free games don't require payment but still need wallet connection.
    /// </remarks>
    [HttpPost("free")]
    [Authorize]  // REQUIRES AUTHENTICATION
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
    /// Create a new paid game lobby (REQUIRES AUTHENTICATION)
    /// </summary>
    [HttpPost("paid")]
    [Authorize]  // REQUIRES AUTHENTICATION
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
    /// Join an existing game lobby (REQUIRES AUTHENTICATION)
    /// </summary>
    [HttpPost("{gameId}/join")]
    [Authorize]  // REQUIRES AUTHENTICATION
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
    /// Submit a move in an active game (REQUIRES AUTHENTICATION)
    /// </summary>
    [HttpPost("{gameId}/moves")]
    [Authorize]  // REQUIRES AUTHENTICATION
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
    /// Complete a game and get final results (REQUIRES AUTHENTICATION)
    /// </summary>
    [HttpPost("{gameId}/complete")]
    [Authorize]  // REQUIRES AUTHENTICATION
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
}