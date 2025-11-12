using Microsoft.EntityFrameworkCore;
using TwinsWins.Core.DTOs;
using TwinsWins.Core.Entities;
using TwinsWins.Core.Enums;
using TwinsWins.Core.Interfaces;
using TwinsWins.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace TwinsWins.Infrastructure.Services;

public class GameService : IGameService
{
    private readonly ApplicationDbContext _context;
    private readonly IAntiCheatService _antiCheatService;
    private readonly IBlockchainService _blockchainService;
    private readonly Dictionary<Guid, GameState> _activeGames = new();

    public GameService(
        ApplicationDbContext context,
        IAntiCheatService antiCheatService,
        IBlockchainService blockchainService)
    {
        _context = context;
        _antiCheatService = antiCheatService;
        _blockchainService = blockchainService;
    }

    public async Task<GameDto> CreateFreeGameAsync(Guid userId, Guid imageSetId)
    {
        var imageSet = await _context.ImageSets
            .Include(s => s.ImagePairs)
            .FirstOrDefaultAsync(s => s.Id == imageSetId && s.IsActive);

        if (imageSet == null)
            throw new InvalidOperationException("Image set not found or inactive");

        if (imageSet.ImagePairs.Count < 9)
            throw new InvalidOperationException("Image set must have at least 9 pairs");

        var game = new Game
        {
            GameType = GameType.Free,
            ImageSetId = imageSetId,
            Status = GameStatus.Active,
            StartedAt = DateTime.UtcNow,
            LayoutHash = GenerateLayoutHash(imageSet.ImagePairs.Take(9).ToList())
        };

        _context.Games.Add(game);

        var participant = new GameParticipant
        {
            GameId = game.Id,
            UserId = userId,
            Role = ParticipantRole.Creator
        };

        _context.GameParticipants.Add(participant);
        await _context.SaveChangesAsync();

        // Initialize game state
        InitializeGameState(game.Id, userId, imageSet.ImagePairs.Take(9).ToList());

        return await MapToGameDto(game);
    }

    public async Task<GameDto> CreatePaidGameAsync(Guid userId, decimal stakeAmount, Guid imageSetId)
    {
        if (stakeAmount <= 0)
            throw new ArgumentException("Stake amount must be greater than zero");

        var imageSet = await _context.ImageSets
            .Include(s => s.ImagePairs)
            .FirstOrDefaultAsync(s => s.Id == imageSetId && s.IsActive);

        if (imageSet == null)
            throw new InvalidOperationException("Image set not found or inactive");

        if (imageSet.ImagePairs.Count < 9)
            throw new InvalidOperationException("Image set must have at least 9 pairs");

        // Create smart contract
        var contractAddress = await _blockchainService.CreateGameContractAsync(Guid.NewGuid(), stakeAmount);

        var game = new Game
        {
            GameType = GameType.Paid,
            StakeAmount = stakeAmount,
            ImageSetId = imageSetId,
            Status = GameStatus.Waiting,
            SmartContractAddress = contractAddress,
            TimeoutAt = DateTime.UtcNow.AddMinutes(10),
            LayoutHash = GenerateLayoutHash(imageSet.ImagePairs.Take(9).ToList())
        };

        _context.Games.Add(game);

        var participant = new GameParticipant
        {
            GameId = game.Id,
            UserId = userId,
            Role = ParticipantRole.Creator
        };

        _context.GameParticipants.Add(participant);

        // Record stake transaction
        var transaction = new Transaction
        {
            WalletAddress = (await _context.Users.FindAsync(userId))!.WalletAddress,
            GameId = game.Id,
            Type = TransactionType.Stake,
            Amount = stakeAmount,
            Status = TransactionStatus.Pending
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return await MapToGameDto(game);
    }

    public async Task<GameDto> JoinGameAsync(Guid gameId, Guid userId)
    {
        var game = await _context.Games
            .Include(g => g.Participants)
            .Include(g => g.ImageSet)
                .ThenInclude(s => s!.ImagePairs)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null)
            throw new InvalidOperationException("Game not found");

        if (game.Status != GameStatus.Waiting)
            throw new InvalidOperationException("Game is not available for joining");

        if (game.Participants.Count >= 2)
            throw new InvalidOperationException("Game is full");

        var participant = new GameParticipant
        {
            GameId = gameId,
            UserId = userId,
            Role = ParticipantRole.Joiner
        };

        _context.GameParticipants.Add(participant);

        // Record stake transaction for joiner
        var transaction = new Transaction
        {
            WalletAddress = (await _context.Users.FindAsync(userId))!.WalletAddress,
            GameId = game.Id,
            Type = TransactionType.Stake,
            Amount = game.StakeAmount!.Value,
            Status = TransactionStatus.Pending
        };

        _context.Transactions.Add(transaction);

        // Update game status
        game.Status = GameStatus.Active;
        game.StartedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Initialize game state for both players
        foreach (var p in game.Participants)
        {
            InitializeGameState(game.Id, p.UserId, game.ImageSet!.ImagePairs.Take(9).ToList());
        }

        return await MapToGameDto(game);
    }

    public async Task<GameResultDto> SubmitMoveAsync(Guid gameId, Guid userId, GameMoveDto move)
    {
        // Anti-cheat validation
        if (!await _antiCheatService.ValidateMoveTimingAsync(gameId, userId, move))
            throw new InvalidOperationException("Invalid move timing detected");

        var game = await _context.Games
            .Include(g => g.Participants)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null || game.Status != GameStatus.Active)
            throw new InvalidOperationException("Game is not active");

        if (!_activeGames.TryGetValue(gameId, out var gameState))
            throw new InvalidOperationException("Game state not found");

        if (!gameState.PlayerStates.TryGetValue(userId, out var playerState))
            throw new InvalidOperationException("Player not in game");

        // Check if cards are already matched
        if (playerState.MatchedPairs.Contains(move.Card1Id / 2) || 
            playerState.MatchedPairs.Contains(move.Card2Id / 2))
        {
            throw new InvalidOperationException("Cards already matched");
        }

        // Check if it's a valid pair
        bool isCorrect = (move.Card1Id / 2) == (move.Card2Id / 2);
        
        int pointsAwarded = CalculatePoints(
            isCorrect, 
            playerState.ConsecutiveCorrectPairs,
            DateTime.UtcNow,
            playerState.GameStartTime
        );

        if (isCorrect)
        {
            playerState.MatchedPairs.Add(move.Card1Id / 2);
            playerState.ConsecutiveCorrectPairs++;
        }
        else
        {
            playerState.ConsecutiveCorrectPairs = 0;
        }

        playerState.TotalScore += pointsAwarded;
        playerState.TotalMoves++;

        // Record move in database
        var gameMove = new GameMove
        {
            GameId = gameId,
            UserId = userId,
            MoveNumber = playerState.TotalMoves,
            Card1Id = move.Card1Id,
            Card2Id = move.Card2Id,
            IsCorrect = isCorrect,
            PointsAwarded = pointsAwarded,
            TimestampMs = move.ClientTimestampMs
        };

        _context.GameMoves.Add(gameMove);
        await _context.SaveChangesAsync();

        bool isGameComplete = playerState.MatchedPairs.Count == 9 || 
                             (DateTime.UtcNow - playerState.GameStartTime).TotalSeconds >= 60;

        return new GameResultDto
        {
            GameId = gameId,
            UserId = userId,
            Score = playerState.TotalScore,
            IsCorrect = isCorrect,
            PointsAwarded = pointsAwarded,
            RemainingPairs = 9 - playerState.MatchedPairs.Count,
            IsGameComplete = isGameComplete
        };
    }

    public async Task<GameResultDto> CompleteGameAsync(Guid gameId, Guid userId)
    {
        var game = await _context.Games
            .Include(g => g.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null)
            throw new InvalidOperationException("Game not found");

        if (!_activeGames.TryGetValue(gameId, out var gameState))
            throw new InvalidOperationException("Game state not found");

        if (!gameState.PlayerStates.TryGetValue(userId, out var playerState))
            throw new InvalidOperationException("Player not in game");

        // Update participant score
        var participant = game.Participants.FirstOrDefault(p => p.UserId == userId);
        if (participant != null)
        {
            participant.Score = playerState.TotalScore;
            participant.CompletedAt = DateTime.UtcNow;
        }

        // Check if all players have completed
        bool allCompleted = game.Participants.All(p => p.CompletedAt != null);

        if (allCompleted && game.GameType == GameType.Paid)
        {
            await SettleGame(game);
        }

        await _context.SaveChangesAsync();

        return new GameResultDto
        {
            GameId = gameId,
            UserId = userId,
            Score = playerState.TotalScore,
            IsGameComplete = true,
            IsWinner = participant?.IsWinner,
            PayoutAmount = participant?.PayoutAmount
        };
    }

    private async Task SettleGame(Game game)
    {
        game.Status = GameStatus.Settling;

        var winner = game.Participants
            .OrderByDescending(p => p.Score)
            .ThenBy(p => p.CompletedAt)
            .First();

        winner.IsWinner = true;

        decimal totalPot = game.StakeAmount!.Value * 2;
        decimal platformFee = totalPot * 0.15m; // 15% platform fee
        decimal affiliateFee = totalPot * 0.03m; // 3% affiliate fee
        decimal winnerPayout = totalPot - platformFee - affiliateFee;

        winner.PayoutAmount = winnerPayout;

        // Process payout via blockchain
        await _blockchainService.ProcessPayoutAsync(
            game.Id,
            winner.User.WalletAddress,
            winnerPayout
        );

        // Handle affiliate payout if applicable
        var affiliateLink = await _context.AffiliateLinks
            .FirstOrDefaultAsync(al => al.ReferredUserId == winner.UserId && al.IsActive);

        if (affiliateLink != null)
        {
            var affiliatePayout = new AffiliatePayout
            {
                LinkId = affiliateLink.Id,
                GameId = game.Id,
                Amount = affiliateFee
            };

            _context.AffiliatePayouts.Add(affiliatePayout);

            affiliateLink.TotalEarnings += affiliateFee;
        }

        game.Status = GameStatus.Settled;
        game.CompletedAt = DateTime.UtcNow;
    }

    public async Task<GameDto> GetGameAsync(Guid gameId)
    {
        var game = await _context.Games
            .Include(g => g.Participants)
                .ThenInclude(p => p.User)
            .Include(g => g.ImageSet)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null)
            throw new InvalidOperationException("Game not found");

        return await MapToGameDto(game);
    }

    public async Task<IEnumerable<GameDto>> GetActiveLobbiesAsync(int skip, int take)
    {
        var games = await _context.Games
            .Include(g => g.Participants)
                .ThenInclude(p => p.User)
            .Include(g => g.ImageSet)
            .Where(g => g.Status == GameStatus.Waiting)
            .OrderByDescending(g => g.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var result = new List<GameDto>();
        foreach (var game in games)
        {
            result.Add(await MapToGameDto(game));
        }

        return result;
    }

    public async Task CancelExpiredGamesAsync()
    {
        var expiredGames = await _context.Games
            .Include(g => g.Participants)
                .ThenInclude(p => p.User)
            .Where(g => g.Status == GameStatus.Waiting && 
                       g.TimeoutAt < DateTime.UtcNow)
            .ToListAsync();

        foreach (var game in expiredGames)
        {
            game.Status = GameStatus.Cancelled;

            // Refund stake to creator
            var creator = game.Participants.First(p => p.Role == ParticipantRole.Creator);
            await _blockchainService.ProcessRefundAsync(
                game.Id,
                creator.User.WalletAddress,
                game.StakeAmount!.Value
            );
        }

        await _context.SaveChangesAsync();
    }

    private int CalculatePoints(bool isCorrect, int consecutiveCorrectPairs, DateTime now, DateTime gameStartTime)
    {
        var elapsedTime = (now - gameStartTime).TotalSeconds;
        var timeBonus = Math.Max(0, (int)((60 - elapsedTime) * 10)); // 0-600 points
        var basePoints = isCorrect ? 100 : -50; // Less harsh penalty
        var comboMultiplier = 1.0 + (0.1 * consecutiveCorrectPairs); // Reward streaks

        if (isCorrect)
        {
            return (int)((basePoints + timeBonus) * comboMultiplier);
        }

        return basePoints; // Negative but capped
    }

    private void InitializeGameState(Guid gameId, Guid userId, List<ImagePair> pairs)
    {
        if (!_activeGames.ContainsKey(gameId))
        {
            _activeGames[gameId] = new GameState();
        }

        _activeGames[gameId].PlayerStates[userId] = new PlayerGameState
        {
            GameStartTime = DateTime.UtcNow,
            MatchedPairs = new HashSet<int>(),
            ConsecutiveCorrectPairs = 0,
            TotalScore = 0,
            TotalMoves = 0
        };
    }

    private string GenerateLayoutHash(List<ImagePair> pairs)
    {
        var layout = string.Join(",", pairs.Select(p => p.Id));
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(layout));
        return Convert.ToBase64String(hashBytes);
    }

    private async Task<GameDto> MapToGameDto(Game game)
    {
        var dto = new GameDto
        {
            Id = game.Id,
            GameType = game.GameType,
            StakeAmount = game.StakeAmount,
            Status = game.Status,
            SmartContractAddress = game.SmartContractAddress,
            CreatedAt = game.CreatedAt,
            StartedAt = game.StartedAt,
            TimeoutAt = game.TimeoutAt,
            Participants = game.Participants.Select(p => new GameParticipantDto
            {
                UserId = p.UserId,
                Username = p.User?.Username,
                WalletAddress = p.User?.WalletAddress ?? "",
                Role = p.Role,
                Score = p.Score,
                IsWinner = p.IsWinner
            }).ToList()
        };

        if (game.ImageSet != null)
        {
            dto.ImageSet = new ImageSetDto
            {
                Id = game.ImageSet.Id,
                Name = game.ImageSet.Name,
                Difficulty = game.ImageSet.Difficulty
            };

            // Generate cards from image pairs
            var pairs = game.ImageSet.ImagePairs.Take(9).ToList();
            var cards = new List<CardDto>();
            int cardId = 0;

            foreach (var pair in pairs)
            {
                cards.Add(new CardDto
                {
                    Id = cardId++,
                    ImageUrl = pair.Image1Path,
                    PairId = pairs.IndexOf(pair)
                });
                cards.Add(new CardDto
                {
                    Id = cardId++,
                    ImageUrl = pair.Image2Path,
                    PairId = pairs.IndexOf(pair)
                });
            }

            // Shuffle cards
            var random = new Random();
            dto.Cards = cards.OrderBy(c => random.Next()).ToList();
        }

        return dto;
    }

    private class GameState
    {
        public Dictionary<Guid, PlayerGameState> PlayerStates { get; set; } = new();
    }

    private class PlayerGameState
    {
        public DateTime GameStartTime { get; set; }
        public HashSet<int> MatchedPairs { get; set; } = new();
        public int ConsecutiveCorrectPairs { get; set; }
        public int TotalScore { get; set; }
        public int TotalMoves { get; set; }
    }
}
