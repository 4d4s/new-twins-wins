using TwinsWins.Core.DTOs;

namespace TwinsWins.BlazorClient.State.Leaderboard;

public class LoadLeaderboardAction
{
    public string Period { get; }
    public string Category { get; }

    public LoadLeaderboardAction(string period, string category)
    {
        Period = period;
        Category = category;
    }
}

public class LoadLeaderboardSuccessAction
{
    public IEnumerable<LeaderboardPlayerDto> Players { get; }
    public LoadLeaderboardSuccessAction(IEnumerable<LeaderboardPlayerDto> players) => Players = players;
}

public class LoadLeaderboardFailureAction
{
    public string ErrorMessage { get; }
    public LoadLeaderboardFailureAction(string errorMessage) => ErrorMessage = errorMessage;
}

public class SetLeaderboardFilterAction
{
    public string Period { get; }
    public string Category { get; }

    public SetLeaderboardFilterAction(string period, string category)
    {
        Period = period;
        Category = category;
    }
}