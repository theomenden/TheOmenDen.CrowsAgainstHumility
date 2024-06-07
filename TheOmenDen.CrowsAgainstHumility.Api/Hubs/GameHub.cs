using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsAgainstHumility.Api.Sagas;
using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;

namespace TheOmenDen.CrowsAgainstHumility.Api.Hubs;

public class GameHub(ILogger<GameHub> logger, GameSagaOrchestrator gameSagaOrchestrator)
    : Hub
{
    public Task StartGame(SessionId sessionId, CancellationToken cancellationToken = default) => gameSagaOrchestrator.StartGameAsync(sessionId, cancellationToken);

    public async Task JoinGame(SessionId sessionId, CancellationToken cancellationToken = default)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId.ToString(), cancellationToken);
        var gameState = await gameSagaOrchestrator.GetGameStateAsync(sessionId, cancellationToken);
        await Clients.Caller.SendAsync("InitializeGameState", gameState, cancellationToken);
    }

    public async Task SendPlayerAction(SessionId sessionId, PlayerAction playerAction, CancellationToken cancellationToken = default)
    {
        var updatedGameState = await gameSagaOrchestrator.ProccessPlayerAction(sessionId, playerAction, cancellationToken);

        await Clients.Group(sessionId.ToString()).SendAsync("GameStateUpdated", updatedGameState, cancellationToken);
    }

    public async Task LeaveGame(SessionId sessionId, CancellationToken cancellationToken = default)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId.ToString(), cancellationToken);
        await Clients.Group(sessionId.ToString()).SendAsync("PlayerLeft", Context.ConnectionId, cancellationToken);
    }
}