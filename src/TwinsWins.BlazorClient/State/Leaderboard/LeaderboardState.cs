using Fluxor;
using TwinsWins.Core.DTOs;

namespace TwinsWins.BlazorClient.State.Leaderboard;

[FeatureState]
public class LeaderboardState
{
    public List<LeaderboardPlayerDto> Players { get; }
    public bool IsLoading { get; }
    public string? ErrorMessage { get; }
    public string SelectedPeriod { get; }
    public string SelectedCategory { get; }

    public LeaderboardState()
    {
        Players = new List<LeaderboardPlayerDto>();
        IsLoading = false;
        ErrorMessage = null;
        SelectedPeriod = "week";
        SelectedCategory = "wins";
    }

    public LeaderboardState(
        List<LeaderboardPlayerDto> players,
        bool isLoading,
        string? errorMessage,
        string selectedPeriod,
        string selectedCategory)
    {
        Players = players;
        IsLoading = isLoading;
        ErrorMessage = errorMessage;
        SelectedPeriod = selectedPeriod;
        SelectedCategory = selectedCategory;
    }
}