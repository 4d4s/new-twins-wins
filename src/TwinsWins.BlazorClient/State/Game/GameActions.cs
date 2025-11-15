using TwinsWins.Core.DTOs;

namespace TwinsWins.BlazorClient.State.Game;

/// <summary>
/// Actions for game state management
/// </summary>

// Load Game Actions
public class LoadGameAction
{
    public Guid GameId { get; }
    public LoadGameAction(Guid gameId) => GameId = gameId;
}

public class LoadGameSuccessAction
{
    public GameDto Game { get; }
    public LoadGameSuccessAction(GameDto game) => Game = game;
}

public class LoadGameFailureAction
{
    public string ErrorMessage { get; }
    public LoadGameFailureAction(string errorMessage) => ErrorMessage = errorMessage;
}

// Create Game Actions
public class CreateFreeGameAction
{
    public Guid ImageSetId { get; }
    public CreateFreeGameAction(Guid imageSetId) => ImageSetId = imageSetId;
}

public class CreatePaidGameAction
{
    public decimal StakeAmount { get; }
    public Guid ImageSetId { get; }
    public CreatePaidGameAction(decimal stakeAmount, Guid imageSetId)
    {
        StakeAmount = stakeAmount;
        ImageSetId = imageSetId;
    }
}

public class CreateGameSuccessAction
{
    public GameDto Game { get; }
    public CreateGameSuccessAction(GameDto game) => Game = game;
}

public class CreateGameFailureAction
{
    public string ErrorMessage { get; }
    public CreateGameFailureAction(string errorMessage) => ErrorMessage = errorMessage;
}

// Join Game Actions
public class JoinGameAction
{
    public Guid GameId { get; }
    public JoinGameAction(Guid gameId) => GameId = gameId;
}

public class JoinGameSuccessAction
{
    public GameDto Game { get; }
    public JoinGameSuccessAction(GameDto game) => Game = game;
}

public class JoinGameFailureAction
{
    public string ErrorMessage { get; }
    public JoinGameFailureAction(string errorMessage) => ErrorMessage = errorMessage;
}

// Load Lobbies Actions
public class LoadLobbiesAction
{
    public int Skip { get; }
    public int Take { get; }
    public LoadLobbiesAction(int skip = 0, int take = 20)
    {
        Skip = skip;
        Take = take;
    }
}

public class LoadLobbiesSuccessAction
{
    public IEnumerable<GameDto> Lobbies { get; }
    public LoadLobbiesSuccessAction(IEnumerable<GameDto> lobbies) => Lobbies = lobbies;
}

public class LoadLobbiesFailureAction
{
    public string ErrorMessage { get; }
    public LoadLobbiesFailureAction(string errorMessage) => ErrorMessage = errorMessage;
}

// Select Card Actions
public class SelectCardAction
{
    public int CardId { get; }
    public SelectCardAction(int cardId) => CardId = cardId;
}

public class ClearSelectedCardsAction { }

// Submit Move Actions
public class SubmitMoveAction
{
    public Guid GameId { get; }
    public GameMoveDto Move { get; }
    public SubmitMoveAction(Guid gameId, GameMoveDto move)
    {
        GameId = gameId;
        Move = move;
    }
}

public class SubmitMoveSuccessAction
{
    public GameResultDto Result { get; }
    public SubmitMoveSuccessAction(GameResultDto result) => Result = result;
}

public class SubmitMoveFailureAction
{
    public string ErrorMessage { get; }
    public SubmitMoveFailureAction(string errorMessage) => ErrorMessage = errorMessage;
}

// Timer Actions
public class UpdateTimerAction
{
    public int TimeRemaining { get; }
    public UpdateTimerAction(int timeRemaining) => TimeRemaining = timeRemaining;
}

// Complete Game Actions
public class CompleteGameAction
{
    public Guid GameId { get; }
    public CompleteGameAction(Guid gameId) => GameId = gameId;
}

public class CompleteGameSuccessAction
{
    public GameResultDto Result { get; }
    public CompleteGameSuccessAction(GameResultDto result) => Result = result;
}

public class CompleteGameFailureAction
{
    public string ErrorMessage { get; }
    public CompleteGameFailureAction(string errorMessage) => ErrorMessage = errorMessage;
}

// Reset Game Action
public class ResetGameStateAction { }