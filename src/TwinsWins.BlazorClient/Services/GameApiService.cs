using System.Net.Http.Json;
using System.Net.Http.Headers;
using TwinsWins.Core.DTOs;

namespace TwinsWins.BlazorClient.Services;

public class GameApiService : IGameApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public GameApiService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<GameDto> CreateFreeGameAsync(Guid imageSetId)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync("/api/games/free", new CreateFreeGameRequest
        {
            ImageSetId = imageSetId
        });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameDto>() 
            ?? throw new Exception("Failed to create game");
    }

    public async Task<GameDto> CreatePaidGameAsync(decimal stakeAmount, Guid imageSetId)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync("/api/games/paid", new CreatePaidGameRequest
        {
            StakeAmount = stakeAmount,
            ImageSetId = imageSetId
        });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameDto>() 
            ?? throw new Exception("Failed to create game");
    }

    public async Task<GameDto> JoinGameAsync(Guid gameId)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PostAsync($"/api/games/{gameId}/join", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameDto>() 
            ?? throw new Exception("Failed to join game");
    }

    public async Task<GameResultDto> SubmitMoveAsync(Guid gameId, GameMoveDto move)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync($"/api/games/{gameId}/moves", move);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameResultDto>() 
            ?? throw new Exception("Failed to submit move");
    }

    public async Task<GameResultDto> CompleteGameAsync(Guid gameId)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PostAsync($"/api/games/{gameId}/complete", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameResultDto>() 
            ?? throw new Exception("Failed to complete game");
    }

    public async Task<GameDto> GetGameAsync(Guid gameId)
    {
        await SetAuthHeaderAsync();
        return await _httpClient.GetFromJsonAsync<GameDto>($"/api/games/{gameId}") 
            ?? throw new Exception("Game not found");
    }

    public async Task<IEnumerable<GameDto>> GetActiveLobbiesAsync(int skip = 0, int take = 20)
    {
        await SetAuthHeaderAsync();
        return await _httpClient.GetFromJsonAsync<IEnumerable<GameDto>>(
            $"/api/games/lobbies?skip={skip}&take={take}") 
            ?? Array.Empty<GameDto>();
    }
}
