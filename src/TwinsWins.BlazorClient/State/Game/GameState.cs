using Fluxor;
using TwinsWins.Core.DTOs;

namespace TwinsWins.BlazorClient.State.Game;

/// <summary>
/// Game State - holds current game data and lobbies
/// </summary>
[FeatureState]
public class GameState
{
    public GameDto? CurrentGame { get; }
    public List<GameDto> Lobbies { get; }
    public bool IsLoading { get; }
    public string? ErrorMessage { get; }
    public int CurrentScore { get; }
    public int TimeRemaining { get; }
    public List<int> SelectedCards { get; }
    public HashSet<int> MatchedPairs { get; }
    public bool IsGameComplete { get; }

    // Default constructor for initial state
    public GameState()
    {
        CurrentGame = null;
        Lobbies = new List<GameDto>();
        IsLoading = false;
        ErrorMessage = null;
        CurrentScore = 0;
        TimeRemaining = 60;
        SelectedCards = new List<int>();
        MatchedPairs = new HashSet<int>();
        IsGameComplete = false;
    }

    // Constructor for state updates
    public GameState(
        GameDto? currentGame,
        List<GameDto> lobbies,
        bool isLoading,
        string? errorMessage,
        int currentScore,
        int timeRemaining,
        List<int> selectedCards,
        HashSet<int> matchedPairs,
        bool isGameComplete)
    {
        CurrentGame = currentGame;
        Lobbies = lobbies;
        IsLoading = isLoading;
        ErrorMessage = errorMessage;
        CurrentScore = currentScore;
        TimeRemaining = timeRemaining;
        SelectedCards = selectedCards;
        MatchedPairs = matchedPairs;
        IsGameComplete = isGameComplete;
    }
}