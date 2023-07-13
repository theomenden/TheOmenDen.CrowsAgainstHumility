using Microsoft.AspNetCore.SignalR.Client;
using TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Constants;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Clients;
public sealed class CrowGameHubClient : ICrowGameHubClient
{
    #region Private Members
    private readonly HubConnection _hubConnection;
    private event Func<Task> Connected;
    #endregion
    #region Constructors
    public CrowGameHubClient(HubConnection hubConnection)
    {
        _hubConnection = hubConnection;
    }
    #endregion
    #region Public Properties
    public string? ConnectionId => _hubConnection.ConnectionId;
    #endregion
    #region Hub Methods
    public async Task Connect(Guid serverId, CancellationToken cancellationToken = default)
    {
        await _hubConnection.InvokeAsync(HubEndpointRoutes.Connect, serverId, cancellationToken);
        await Connected.Invoke();
    }
    public Task Disconnect(CancellationToken cancellationToken = default) => _hubConnection.StopAsync(cancellationToken);
    public Task ClearBoard(Guid serverId, CancellationToken cancellationToken = default) => _hubConnection.InvokeAsync(HubEndpointRoutes.Clear, serverId, cancellationToken);
    public Task<ServerCreationResult> CreateServer(IEnumerable<Pack> cardSet, CancellationToken cancellationToken = default) => _hubConnection.InvokeAsync<ServerCreationResult>(HubEndpointRoutes.Create, cardSet, cancellationToken);
    public Task<PlayerViewModel> JoinServer(Guid serverId, Guid recoveryId, string playerName, GameRoles playerType, CancellationToken cancellationToken = default) => _hubConnection.InvokeAsync<PlayerViewModel>(HubEndpointRoutes.Join, serverId, recoveryId, playerName, playerType, cancellationToken);
    public Task KickPlayer(Guid serverId, string initiatingPlayerConnectionId, int kickedPlayerPublicId, CancellationToken cancellationToken = default) => _hubConnection.InvokeAsync(HubEndpointRoutes.Kick, serverId, initiatingPlayerConnectionId, kickedPlayerPublicId, cancellationToken);
    public Task ShowPlayedCards(Guid serverId, CancellationToken cancellationToken = default) => _hubConnection.InvokeAsync(HubEndpointRoutes.Show, serverId, cancellationToken);
    public Task PlayCard(Guid serverId, string playerConnectionId, WhiteCard card, CancellationToken cancellationToken = default) => _hubConnection.InvokeAsync(HubEndpointRoutes.PlayCard, serverId, playerConnectionId, card, cancellationToken);
    public Task UnPlayCard(Guid serverId, string playerConnectionId, CancellationToken cancellationToken = default) => _hubConnection.InvokeAsync(HubEndpointRoutes.UnPlayCard, serverId, cancellationToken);
    public Task<PlayerViewModel> ChangePlayerType(Guid serverId, GameRoles newType, CancellationToken cancellationToken = default) => _hubConnection.InvokeAsync<PlayerViewModel>(HubEndpointRoutes.ChangePlayerType, serverId, newType, cancellationToken);
    #endregion
    #region Hub Event Registration Methods
    public void OnSessionUpdated(Action<CrowGameServerViewModel> onSessionUpdatedHandler) => _hubConnection.On(BroadcastChannels.UPDATED, onSessionUpdatedHandler);
    public void OnPlayerKicked(Action<PlayerViewModel> onPlayerKickedHandler) => _hubConnection.On(BroadcastChannels.KICKED, onPlayerKickedHandler);
    public void OnLogMessageReceived(Action<GameMessage> onLogMessageReceivedHandler) => _hubConnection.On(BroadcastChannels.LOG, onLogMessageReceivedHandler);
    public void OnGameBoardCleared(Action onGameBoardClearedHandler) => _hubConnection.On(BroadcastChannels.CLEAR, onGameBoardClearedHandler);
    public void OnReconnected(Func<string, Task> reconnectedHandler) => _hubConnection.Reconnected += reconnectedHandler;
    public void OnReconnecting(Func<Exception, Task> reconnectingHandler) => _hubConnection.Reconnecting += reconnectingHandler;
    public void OnClosed(Func<Exception, Task> closedHandler) => _hubConnection.Closed += closedHandler;
    public void OnConnected(Func<Task> connectedHandler) => Connected += connectedHandler;
    #endregion
}
