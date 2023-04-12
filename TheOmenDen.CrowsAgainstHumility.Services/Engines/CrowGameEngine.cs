using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Exceptions;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Engines;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
using TheOmenDen.CrowsAgainstHumility.Services.Decks;
using TheOmenDen.CrowsAgainstHumility.Services.Managers;
using Guard = TheOmenDen.Shared.Guards.Guard;

namespace TheOmenDen.CrowsAgainstHumility.Services.Engines;
internal class CrowGameEngine : ICrowGameEngine
{
    #region Constants
    private const int MaximumPlayerNameLength = 30;
    #endregion  
    #region Event Handlers
    public event EventHandler<PlayerKickedEventArgs>? PlayerKicked;
    public event EventHandler<RoomUpdatedEventArgs>? RoomUpdated;
    public event EventHandler<LogUpdatedEventArgs>? LogUpdated;
    public event EventHandler<RoomClearedEventArgs>? RoomCleared;
    #endregion
    #region Injected members
    private ICrowGameServerStore _serverStore;
    private ILogger<CrowGameEngine> _logger;
    #endregion
    #region Constructors
    public CrowGameEngine(ICrowGameServerStore serverStore, ILogger<CrowGameEngine> logger)
    {
        _serverStore = serverStore;
        _logger = logger;
    }
    #endregion
    #region Synchronous Methods
    public void Kick(Guid id, string initiatingPlayerPrivateId, int playerPublicIdToRemove)
    {
        var server = _serverStore.GetById(id);

        var player = ServerManager.GetPlayer(server, initiatingPlayerPrivateId);

        var (wasRemoved, kickedPlayer) = ServerManager.TryRemovePlayer(server, playerPublicIdToRemove);

        if (!wasRemoved || kickedPlayer is null)
        {
            return;
        }

        RaisePlayerKickedEvent(id, kickedPlayer);
        RaiseRoomUpdatedEvent(id, server);
        RaiseLogUpdatedEvent(id, player.Name, $"Kicked {kickedPlayer.Name}");
    }

    public void SleepInAllRooms(string playerPrivateId)
    {
        var serversWithPlayer = ServerManager.SetPlayerToSleepOnAllServers(_serverStore.GetAll(), playerPrivateId);

        foreach (var crowGameServer in serversWithPlayer)
        {
            RaiseRoomUpdatedEvent(crowGameServer.Id, crowGameServer);
        }
    }

    public (bool wasCreated, Guid? serverId, string? validationMessages) CreateRoom(Deck deck)
    {
        var (isParsed, cards, validationMessage) = DeckProcessor.TryParseCardSet(deck);

        if (!isParsed)
        {
            return (false, null, "Deck could not be parsed for gameplay. Please create a valid deck.");
        }

        var server = _serverStore.Create(cards);
        return (true, server.Id);
    }

    public Player JoinRoom(Guid id, Guid recoveryId, string playerName, string playerPrivateId, GameRoles playerType)
    {
        Guard.FromNullOrWhitespace(playerName, nameof(playerName));

        var server = _serverStore.GetById(id);

        var formattedPlayerName = playerName.Length > MaximumPlayerNameLength
            ? playerName[..MaximumPlayerNameLength]
            : playerName;

        var newPlayer = ServerManager.AddOrUpdatePlayer(server, recoveryId, playerPrivateId, formattedPlayerName, playerType);

        RaiseRoomUpdatedEvent(id, server);
        RaiseLogUpdatedEvent(id, newPlayer.Username, "Joined the server");
        return newPlayer;
    }

    public void PlayWhiteCard(Guid serverId, string playerPrivateId, WhiteCard playedCard)
    {
        var server = _serverStore.GetById(serverId);

        if (!server.CurrentSession.GetWhiteCards().Contains(playedCard))
        {
            throw new CannotPlayCardException("Card does not exist in playable card set");
        }

        if (!server.CurrentSession.CanPlayWhiteCard)
        {
            throw new CannotPlayCardException("Session not in a state where players can play cards");
        }

        if (!server.Players.ContainsKey(playerPrivateId))
        {
            throw new CannotPlayCardException("Player is not part of the current session.");
        }

        var player = ServerManager.GetPlayer(server, playerPrivateId);

        if (player.GameRole == GameRoles.Observer)
        {
            throw new CannotPlayCardException($"Player is of type '{GameRoles.Observer}' and cannot playe a card.");
        }

        if (player.Mode == PlayerMode.Sleep)
        {
            throw new CannotPlayCardException($"Player is {player.Mode} and cannot play a card.");
        }

        SessionManager.PlayWhiteCard(session, player.PublicId, playedCard);
        RaiseRoomUpdatedEvent(server.Id, server);
        RaiseLogUpdatedEvent(server.Id, player.Name, "Played Card.");
    }

    public void RedactPlayedWhiteCard(Guid serverId, string playerPrivateId)
    {
        var server = _serverStore.GetById(serverId);

        if (!server.CurrentSession.CanPlayWhiteCard)
        {
            throw new CannotRedactCardException("Session is not in a state where players can redact their played cards.");
        }

        if (!server.Players.ContainsKey(playerPrivateId))
        {
            throw new CannotRedactCardException("Player is not a part of the current session.");
        }

        var player = ServerManager.GetPlayer(server, playerPrivateId);

        if (player.GameRole == GameRoles.Observer)
        {
            throw new CannotRedactCardException($"Player is of type {player.GameRole} amd cannot play a card.");
        }

        if (player.Mode == PlayerMode.Sleep)
        {
            throw new CannotRedactCardException($"Player is currently in {player.Mode}, and cannot vote.");
        }

        SessionManager.RemovePlayedCard(server.CurrentSession, player.PublicId);
        RaiseRoomUpdatedEvent(server.Id, server);
        RaiseLogUpdatedEvent(server.Id, player.Username, "UnPlayed their card");
    }

    public void ClearGameBoard(Guid serverId, string playerPrivateId)
    {
        var server = _serverStore.GetById(serverId);

        if (!server.CurrentSession.CanClear)
        {
            throw new CannotClearBoardException("Session not in a state where game board can be cleared");
        }

        SessionManager.ClearPlayedCards(server.CurrentSession);
        var player = ServerManager.GetPlayer(server, playerPrivateId);

        RaiseRoomUpdatedEvent(server.Id, server);
        RaiseRoomClearedEvent(server.Id);
        RaiseLogUpdatedEvent(server.Id, player.Username, "Cleared all played cards");
    }

    public void ShowWhiteCards(Guid serverId, string playerPrivateId)
    {
        var server = _serverStore.GetById(serverId);

        var player = ServerManager.GetPlayer(server, playerPrivateId);

        if (!server.CurrentSession.CanShow(server.Players)
            || !player.IsCardCzar)
        {
            throw new CannotShowCardsException("Current session is not in a state where cards can be shown");
        }

        SessionManager.ShowWhiteCards(server.CurrentSession);
        RaiseRoomUpdatedEvent(server.Id, server);
        RaiseLogUpdatedEvent(server.Id, player.Username, "Made all played cards visible.");
    }

    public Player ChangePlayerType(Guid serverId, string playerPrivateId, GameRoles newPlayerType)
    {
        var server = _serverStore.GetById(serverId);
        var player = ServerManager.GetPlayer(server, playerPrivateId);

        if (SessionManager.HasPlayedACard(server.CurrentSession, player.PublicId))
        {
            throw new CannotChangePlayerTypeException($"Cannot change from Role while '{GameRoles.Player}' has played a card");
        }

        player = ServerManager.ChangePlayerType(server, player, newPlayerType);
        RaiseRoomUpdatedEvent(server.Id, server);
        RaiseLogUpdatedEvent(server.Id, player.Name, $"Changed their player type to {newPlayerType}");
        return player;
    }
    #endregion
    #region Async Methods
    public async Task KickAsync(Guid id, string initiatingPlayerPrivateId, int playerPublicIdToRemove,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task SleepInAllRoomsAsync(string playerPrivateId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<(bool wasCreated, Guid? serverId, string? validationMessages)> CreateRoomAsync(IEnumerable<Pack> packs, CancellationToken cancellationToken = default)
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
    #region Private Event Raising Methods
    private void RaiseRoomClearedEvent(Guid serverId) => RoomCleared?.Invoke(this, new RoomClearedEventArgs(serverId));

    private void RaiseLogUpdatedEvent(Guid serverId, string initiatingPlayerName, string logMessage)
        => LogUpdated?.Invoke(this, new LogUpdatedEventArgs(serverId, initiatingPlayerName, logMessage));

    private void RaiseRoomUpdatedEvent(Guid serverId, CrowGameServer updatedServer)
        => RoomUpdated?.Invoke(this, new RoomUpdatedEventArgs(serverId, updatedServer));

    private void RaisePlayerKickedEvent(Guid serverId, Player kickedPlayer)
        => PlayerKicked?.Invoke(this, new PlayerKickedEventArgs(serverId, kickedPlayer));
    #endregion
}
