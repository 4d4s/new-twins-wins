using Fluxor;
using TwinsWins.Core.DTOs;

namespace TwinsWins.BlazorClient.State.Leaderboard;

public static class LeaderboardReducers
{
    [ReducerMethod]
    public static LeaderboardState ReduceLoadLeaderboardAction(LeaderboardState state, LoadLeaderboardAction action)
    {
        return new LeaderboardState(
            players: state.Players,
            isLoading: true,
            errorMessage: null,
            selectedPeriod: action.Period,
            selectedCategory: action.Category
        );
    }

    [ReducerMethod]
    public static LeaderboardState ReduceLoadLeaderboardSuccessAction(LeaderboardState state, LoadLeaderboardSuccessAction action)
    {
        return new LeaderboardState(
            players: action.Players.ToList(),
            isLoading: false,
            errorMessage: null,
            selectedPeriod: state.SelectedPeriod,
            selectedCategory: state.SelectedCategory
        );
    }

    [ReducerMethod]
    public static LeaderboardState ReduceLoadLeaderboardFailureAction(LeaderboardState state, LoadLeaderboardFailureAction action)
    {
        return new LeaderboardState(
            players: new List<LeaderboardPlayerDto>(),
            isLoading: false,
            errorMessage: action.ErrorMessage,
            selectedPeriod: state.SelectedPeriod,
            selectedCategory: state.SelectedCategory
        );
    }

    [ReducerMethod]
    public static LeaderboardState ReduceSetLeaderboardFilterAction(LeaderboardState state, SetLeaderboardFilterAction action)
    {
        return new LeaderboardState(
            players: state.Players,
            isLoading: state.IsLoading,
            errorMessage: state.ErrorMessage,
            selectedPeriod: action.Period,
            selectedCategory: action.Category
        );
    }
}