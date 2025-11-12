using Fluxor;

namespace TwinsWins.BlazorClient.State.Game;

/// <summary>
/// Reducers for game state - pure functions that create new state
/// </summary>
public static class GameReducers
{
    [ReducerMethod]
    public static GameState ReduceLoadGameAction(GameState state, LoadGameAction action)
    {
        return new GameState(
            currentGame: null,
            isLoading: true,
            errorMessage: null,
            currentScore: state.CurrentScore,
            timeRemaining: state.TimeRemaining,
            selectedCards: state.SelectedCards,
            matchedPairs: state.MatchedPairs,
            isGameComplete: state.IsGameComplete
        );
    }

    [ReducerMethod]
    public static GameState ReduceLoadGameSuccessAction(GameState state, LoadGameSuccessAction action)
    {
        return new GameState(
            currentGame: action.Game,
            isLoading: false,
            errorMessage: null,
            currentScore: 0,
            timeRemaining: 60,
            selectedCards: new List<int>(),
            matchedPairs: new HashSet<int>(),
            isGameComplete: false
        );
    }

    [ReducerMethod]
    public static GameState ReduceLoadGameFailureAction(GameState state, LoadGameFailureAction action)
    {
        return new GameState(
            currentGame: null,
            isLoading: false,
            errorMessage: action.ErrorMessage,
            currentScore: state.CurrentScore,
            timeRemaining: state.TimeRemaining,
            selectedCards: state.SelectedCards,
            matchedPairs: state.MatchedPairs,
            isGameComplete: state.IsGameComplete
        );
    }

    [ReducerMethod]
    public static GameState ReduceCreateGameSuccessAction(GameState state, CreateGameSuccessAction action)
    {
        return new GameState(
            currentGame: action.Game,
            isLoading: false,
            errorMessage: null,
            currentScore: 0,
            timeRemaining: 60,
            selectedCards: new List<int>(),
            matchedPairs: new HashSet<int>(),
            isGameComplete: false
        );
    }

    [ReducerMethod]
    public static GameState ReduceSelectCardAction(GameState state, SelectCardAction action)
    {
        var newSelectedCards = new List<int>(state.SelectedCards) { action.CardId };
        
        return new GameState(
            currentGame: state.CurrentGame,
            isLoading: state.IsLoading,
            errorMessage: state.ErrorMessage,
            currentScore: state.CurrentScore,
            timeRemaining: state.TimeRemaining,
            selectedCards: newSelectedCards,
            matchedPairs: state.MatchedPairs,
            isGameComplete: state.IsGameComplete
        );
    }

    [ReducerMethod]
    public static GameState ReduceClearSelectedCardsAction(GameState state, ClearSelectedCardsAction action)
    {
        return new GameState(
            currentGame: state.CurrentGame,
            isLoading: state.IsLoading,
            errorMessage: state.ErrorMessage,
            currentScore: state.CurrentScore,
            timeRemaining: state.TimeRemaining,
            selectedCards: new List<int>(),
            matchedPairs: state.MatchedPairs,
            isGameComplete: state.IsGameComplete
        );
    }

    [ReducerMethod]
    public static GameState ReduceSubmitMoveSuccessAction(GameState state, SubmitMoveSuccessAction action)
    {
        var newMatchedPairs = new HashSet<int>(state.MatchedPairs);
        
        if (action.Result.IsCorrect)
        {
            // Add the pair to matched pairs
            var card1Pair = state.SelectedCards[0] / 2;
            newMatchedPairs.Add(card1Pair);
        }

        return new GameState(
            currentGame: state.CurrentGame,
            isLoading: false,
            errorMessage: null,
            currentScore: action.Result.Score,
            timeRemaining: state.TimeRemaining,
            selectedCards: new List<int>(),
            matchedPairs: newMatchedPairs,
            isGameComplete: action.Result.IsGameComplete
        );
    }

    [ReducerMethod]
    public static GameState ReduceUpdateTimerAction(GameState state, UpdateTimerAction action)
    {
        return new GameState(
            currentGame: state.CurrentGame,
            isLoading: state.IsLoading,
            errorMessage: state.ErrorMessage,
            currentScore: state.CurrentScore,
            timeRemaining: action.TimeRemaining,
            selectedCards: state.SelectedCards,
            matchedPairs: state.MatchedPairs,
            isGameComplete: state.IsGameComplete || action.TimeRemaining <= 0
        );
    }

    [ReducerMethod]
    public static GameState ReduceCompleteGameSuccessAction(GameState state, CompleteGameSuccessAction action)
    {
        return new GameState(
            currentGame: state.CurrentGame,
            isLoading: false,
            errorMessage: null,
            currentScore: action.Result.Score,
            timeRemaining: 0,
            selectedCards: state.SelectedCards,
            matchedPairs: state.MatchedPairs,
            isGameComplete: true
        );
    }

    [ReducerMethod]
    public static GameState ReduceResetGameStateAction(GameState state, ResetGameStateAction action)
    {
        return new GameState(); // Return initial state
    }
}
