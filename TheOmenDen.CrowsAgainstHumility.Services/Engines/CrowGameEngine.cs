using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Engines;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
using TheOmenDen.CrowsAgainstHumility.Services.Managers;

namespace TheOmenDen.CrowsAgainstHumility.Services.Engines;
internal class CrowGameEngine : ICrowGameEngine
{
    #region Constants
    private const int MaxPlayerNameLength = 50;
    #endregion
    #region Private Members
    private readonly IServerStore _serverStore;
    private readonly ILogger<CrowGameEngine> _logger;
    #endregion
    #region Event Handlers
    public event EventHandler<PlayerKickedEventArgs>? PlayerKicked;
    public event EventHandler<RoomUpdatedEventArgs>? RoomUpdated;
    public event EventHandler<LogUpdatedEventArgs>? LogUpdated;
    public event EventHandler<RoomClearedEventArgs>? RoomCleared;
    #endregion
    #region Constructors
    public CrowGameEngine(IServerStore serverStore, ILogger<CrowGameEngine> logger)
    {
        _serverStore = serverStore;
        _logger = logger;
    }
    #endregion
    #region Synchronous Methods
    public void Kick(Guid id, string initiatingPlayerPrivateId, int playerPublicIdToRemove)
    {
        var server = _serverStore.Get(id);
        var player = CrowGameServerManager.GetPlayer(server, initiatingPlayerPrivateId);
        var (wasRemoved, kickedPlayer) = CrowGameServerManager.TryRemovePlayer(server, playerPublicIdToRemove);

        if (!wasRemoved || kickedPlayer is null)
        {
            return;
        }

        RaisePlayerKicked(id, kickedPlayer);
        RaiseRoomUpdated(id, server);
        RaiseLogUpdated(id, kickedPlayer.Username, $"Kicked {kickedPlayer.Username}.");
    }

    public void SleepInAllRooms(string playerPrivateId)
    {
        var serversWithPlayer = CrowGameServerManager.SetPlayerToSleepOnAllServers(_serverStore.GetAll(), playerPrivateId);

        foreach (var server in serversWithPlayer)
        {
            RaiseRoomUpdated(server.Id, server);
        }
    }

    public (bool wasCreated, Guid? serverId, string? validationMessages) CreateRoom(IEnumerable<Pack> packs)
    {
        throw new NotImplementedException();
    }

    public Player JoinRoom(Guid id, Guid recoveryId, string playerName, string playerPrivateId, GameRoles playerType)
    {
        throw new NotImplementedException();
    }

    public void PlayWhiteCard(Guid serverId, string playerPrivateId)
    {
        throw new NotImplementedException();
    }

    public void ClearGameBoard(Guid serverId, string playerPrivateId)
    {
        throw new NotImplementedException();
    }

    public void ShowWhiteCards(Guid serverId, string playerPrivateId)
    {
        throw new NotImplementedException();
    }

    public Player ChangePlayerType(Guid serverId, string playerPrivateId, GameRoles newPlayerType)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region Async Methods
    public async Task KickAsync(Guid id, string initiatingPlayerPrivateId, int playerPublicIdToRemove,
        CancellationToken cancellationToken = default)
    {
        var server = await _serverStore.GetByIdAsync(id);
        var player = CrowGameServerManager.GetPlayer(server, initiatingPlayerPrivateId);
        var (wasRemoved, kickedPlayer) = CrowGameServerManager.TryRemovePlayer(server, playerPublicIdToRemove);

        if (!wasRemoved || kickedPlayer is null)
        {
            return;
        }

        RaisePlayerKicked(id, kickedPlayer);
        RaiseRoomUpdated(id, server);
        RaiseLogUpdated(id, kickedPlayer.Username, $"Kicked {kickedPlayer.Username}.");
    }

    public async Task SleepInAllRoomsAsync(string playerPrivateId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<(bool wasCreated, Guid? serverId, string? validationMessages)> CreateRoomAsync(IList<WhiteCard> whiteCards, IList<BlackCard> blackCards, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Player> JoinRoomAsync(Guid id, Guid recoveryId, string playerName, string playerPrivateId, GameRoles playerType,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task PlayWhiteCardAsync(Guid serverId, string playerPrivateId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task ClearGameBoardAsync(Guid serverId, string playerPrivateId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task ShowWhiteCardsAsync(Guid serverId, string playerPrivateId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Player> ChangePlayerTypeAsync(Guid serverId, string playerPrivateId, GameRoles newPlayerType,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region Private Methods
    private void RaiseRoomCleared(Guid serverId)
        => RoomCleared?.Invoke(this, new RoomClearedEventArgs(serverId));

    private void RaiseLogUpdated(Guid serverId, string initiatingPlayerName, string logMessage)
        => LogUpdated?.Invoke(this, new LogUpdatedEventArgs(serverId, initiatingPlayerName, logMessage));

    private void RaiseRoomUpdated(Guid serverId, CrowGameServer updatedServer)
        => RoomUpdated?.Invoke(this, new RoomUpdatedEventArgs(serverId, updatedServer));

    private void RaisePlayerKicked(Guid serverId, Player kickedPlayer)
        => PlayerKicked?.Invoke(this, new PlayerKickedEventArgs(serverId, kickedPlayer));
    #endregion
}
