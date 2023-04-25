using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Clients;

public interface ICrowGameHubClient
{
    string? ConnectionId { get; }
    Task Connect(Guid serverId, CancellationToken cancellationToken = default);
    Task Disconnect(CancellationToken cancellationToken = default);
    Task ClearBoard(Guid serverId, CancellationToken cancellationToken = default);
    Task<ServerCreationResult> CreateServer(IEnumerable<Pack> cardSet, CancellationToken cancellationToken = default);
    Task<PlayerViewModel> JoinServer(Guid serverId, Guid recoveryId, string playerName, GameRoles playerType, CancellationToken cancellationToken = default);

    Task KickPlayer(Guid serverId, string initiatingPlayerConnectionId, int kickedPlayerPublicId,
        CancellationToken cancellationToken = default);

    Task ShowPlayedCards(Guid serverId, CancellationToken cancellationToken = default);

    Task PlayCard(Guid serverId, string playerConnectionId, WhiteCard card,
        CancellationToken cancellationToken = default);

    Task UnPlayCard(Guid serverId, string playerConnectionId, CancellationToken cancellationToken = default);
    Task<PlayerViewModel> ChangePlayerType(Guid serverId, GameRoles newType, CancellationToken cancellationToken = default);
    void OnSessionUpdated(Action<CrowGameServerViewModel> onSessionUpdatedHandler);
    void OnPlayerKicked(Action<PlayerViewModel> onPlayerKickedHandler);
    void OnLogMessageReceived(Action<LogMessage> onLogMessageReceivedHandler);
    void OnGameBoardCleared(Action onGameBoardClearedHandler);
    void OnReconnected(Func<string, Task> reconnectedHandler);
    void OnReconnecting(Func<Exception, Task> closedHandler);
    void OnClosed(Func<Exception, Task> closedHandler);
    void OnConnected(Func<Task> connectedHandler);
}