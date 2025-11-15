using Fluxor;

namespace TwinsWins.BlazorClient.State.Leaderboard;

public class LeaderboardEffects
{
    // TODO: Inject ILeaderboardApiService when available
    
    [EffectMethod]
    public async Task HandleLoadLeaderboardAction(LoadLeaderboardAction action, IDispatcher dispatcher)
    {
        try
        {
            // TODO: Replace with actual API call
            await Task.Delay(500);
            
            // Mock data for now
            var players = new List<Core.DTOs.LeaderboardPlayerDto>();
            
            dispatcher.Dispatch(new LoadLeaderboardSuccessAction(players));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadLeaderboardFailureAction(ex.Message));
        }
    }
}