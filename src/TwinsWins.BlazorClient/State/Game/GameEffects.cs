using Fluxor;
using TwinsWins.BlazorClient.Services;

namespace TwinsWins.BlazorClient.State.Game;

/// <summary>
/// Effects handle side effects (async operations, API calls)
/// </summary>
public class GameEffects
{
    private readonly IGameApiService _gameApi;

    public GameEffects(IGameApiService gameApi)
    {
        _gameApi = gameApi;
    }

    [EffectMethod]
    public async Task HandleLoadGameAction(LoadGameAction action, IDispatcher dispatcher)
    {
        try
        {
            var game = await _gameApi.GetGameAsync(action.GameId);
            dispatcher.Dispatch(new LoadGameSuccessAction(game));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadGameFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleCreateFreeGameAction(CreateFreeGameAction action, IDispatcher dispatcher)
    {
        try
        {
            var game = await _gameApi.CreateFreeGameAsync(action.ImageSetId);
            dispatcher.Dispatch(new CreateGameSuccessAction(game));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new CreateGameFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleCreatePaidGameAction(CreatePaidGameAction action, IDispatcher dispatcher)
    {
        try
        {
            var game = await _gameApi.CreatePaidGameAsync(action.StakeAmount, action.ImageSetId);
            dispatcher.Dispatch(new CreateGameSuccessAction(game));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new CreateGameFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleJoinGameAction(JoinGameAction action, IDispatcher dispatcher)
    {
        try
        {
            var game = await _gameApi.JoinGameAsync(action.GameId);
            dispatcher.Dispatch(new JoinGameSuccessAction(game));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new JoinGameFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleLoadLobbiesAction(LoadLobbiesAction action, IDispatcher dispatcher)
    {
        try
        {
            var lobbies = await _gameApi.GetActiveLobbiesAsync(action.Skip, action.Take);
            dispatcher.Dispatch(new LoadLobbiesSuccessAction(lobbies));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadLobbiesFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleSubmitMoveAction(SubmitMoveAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _gameApi.SubmitMoveAsync(action.GameId, action.Move);
            dispatcher.Dispatch(new SubmitMoveSuccessAction(result));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new SubmitMoveFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleCompleteGameAction(CompleteGameAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _gameApi.CompleteGameAsync(action.GameId);
            dispatcher.Dispatch(new CompleteGameSuccessAction(result));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new CompleteGameFailureAction(ex.Message));
        }
    }
}