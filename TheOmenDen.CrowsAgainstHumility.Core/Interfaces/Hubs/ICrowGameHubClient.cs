using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Results;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Hubs;
public interface ICrowGameHubClient
{
    #region Public Properties
    string? ConnectionId { get; }
    #endregion
    #region Async Methods
    Task Connect(Guid serverId, CancellationToken cancellationToken = default);
    Task Disconnect(CancellationToken cancellationToken = default);
    Task ClearGameBoard(Guid serverId, CancellationToken cancellationToken = default);
    Task<ServerCreationResult> CreateServer(IEnumerable<Pack> packs, CancellationToken cancellationToken = default);
    Task KickPlayer(Guid serverId, string initiatingPlayerPrivateId, int kickedPlayerPublicId, CancellationToken cancellationToken = default);
    Task ShowPlayedWhiteCards(Guid serverId, CancellationToken cancellationToken = default);
    Task UnPlayCard(Guid serverId, string playerPrivateId, CancellationToken cancellationToken = default);
    Task PlayCard(Guid serverId, string playerPrivateId, WhiteCard card, CancellationToken cancellationToken = default);
    Task ChooseWinningCard(Guid serverId, string playerPrivateId, WhiteCard card, CancellationToken cancellationToken = default);
    #endregion
    #region Synchronous Methods

    void OnSessionUpdated(Action<CrowGameServerViewModel> onSessionUpdatedHandler);
    void OnPlayerKicked(Action<PlayerViewModel> onPlayerKickedHandler);
    void OnLogMessageReceived(Action<LogMessage> onLogMessageReceivedHandler);
    void OnGameBoardCleared(Action onGameBoardClearedHandler);
    void OnReconnected(Func<string, Task> reconnectedHandler);
    void OnReconnecting(Func<Exception, Task> reconnectingHandler);
    void OnClosed(Func<Exception, Task> closedHandler);
    void OnConnected(Func<Task> connectedHandler);
    #endregion
}
