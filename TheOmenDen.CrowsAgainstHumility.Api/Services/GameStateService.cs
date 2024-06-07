using EasyCaching.Core;
using FastEndpoints;
using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Api.Services;

public interface IGameStateService
{
    Task<GameSessionDto> InitializeGameStateAsync(SessionId sessionId, CancellationToken cancellationToken = default);
    Task<GameSessionDto> GetGameStateAsync(SessionId sessionId, CancellationToken cancellationToken = default);
    Task SaveGameStateAsync(GameSessionDto gameState, CancellationToken cancellationToken = default);
    Task UpdateGameStateAsync(SessionId sessionId, PlayerAction playerAction, CancellationToken cancellationToken = default);
}

[RegisterService<IGameStateService>(LifeTime.Scoped)]
internal sealed class GameStateService(IEasyCachingProvider provider, IGameRepository gameRepository)
: IGameStateService
{
    public async Task<GameSessionDto> InitializeGameStateAsync(SessionId sessionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<GameSessionDto> GetGameStateAsync(SessionId sessionId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"gamestate-{sessionId}";

        return provider.GetAsync<GameSessionDto>(
            cacheKey,
            async () => await gameRepository.GetGameStateAsync(sessionId),
            TimeSpan.FromSeconds(600),
            cancellationToken);
    }


    public async Task SaveGameStateAsync(GameSessionDto gameState, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"gamestate-{gameState.Id}";

        await gameRepository.SaveGameStateAsync(gameState, cancellationToken);
        await provider.SetAsync(cacheKey, gameState, TimeSpan.FromSeconds(600), cancellationToken);
    }

    public async Task UpdateGameStateAsync(SessionId sessionId, PlayerAction playerAction, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"gamestate-{sessionId}";

        var gameState = await GetGameStateAsync(sessionId, cancellationToken);

        if (gameState is null)
        {
            return;
        }

        var updatedGameState = ApplyActionToGameState(gameState, playerAction);
        await gameRepository.UpdateGameStateAsync(updatedGameState);
        await provider.SetAsync(cacheKey, updatedGameState, TimeSpan.FromSeconds(600), cancellationToken);
    }

    private GameSessionDto ApplyActionToGameState(GameSessionDto gameState, PlayerAction playerAction)
    {
        return gameState with
        {
            // Apply the player action to the game state
        };
    }

}