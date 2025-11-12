using Fluxor;
using TwinsWins.BlazorClient.Services;
using TwinsWins.Core.DTOs;

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
    public async Task HandleSubmitMoveAction(SubmitMoveAction action, IDispatcher dispatcher)
    {
        try
        {
            var gameId = action.Move.Card1Id; // TODO: Get actual game ID from state
            // This is a workaround - you'd need to get gameId from the current state
            // For now, we'll need to pass it differently or store it in the move
            
            // var result = await _gameApi.SubmitMoveAsync(gameId, action.Move);
            // dispatcher.Dispatch(new SubmitMoveSuccessAction(result));
            
            // Temporary: dispatch success with mock data
            var mockResult = new GameResultDto
            {
                IsCorrect = true,
                Score = 100,
                PointsAwarded = 100,
                IsGameComplete = false
            };
            dispatcher.Dispatch(new SubmitMoveSuccessAction(mockResult));
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
            // TODO: Get game ID from state
            // var result = await _gameApi.CompleteGameAsync(gameId);
            // dispatcher.Dispatch(new CompleteGameSuccessAction(result));
            
            await Task.CompletedTask; // Placeholder
        }
        catch (Exception ex)
        {
            // Handle error
        }
    }
}
