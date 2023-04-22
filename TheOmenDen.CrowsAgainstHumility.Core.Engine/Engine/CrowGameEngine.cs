using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Events;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Exceptions;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Managers;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Processors;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Stores;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Engine;
internal sealed class CrowGameEngine : ICrowGameEngine
{
    #region Private fields
    private const int MaximumPlayerNameLength = 30;
    private readonly IServerStore _store;
    #endregion
    #region Constructors
    public CrowGameEngine(IServerStore store)
    {
        _store = store;
    }
    #endregion
    #region Events
    public event EventHandler<PlayerKickedEventArgs> PlayerKicked;
    public event EventHandler<RoomUpdatedEventArgs> RoomUpdated;
    public event EventHandler<LogUpdatedEventArgs> LogUpdated;
    public event EventHandler<RoomClearedEventArgs> RoomCleared;
    #endregion
    #region Public Methods
    public async Task KickAsync(Guid serverId, string initiatingPlayerConnectionId, int playerToRemovePublicId, CancellationToken cancellationToken = default)
    {
        var server = await _store.GetServerByIdAsync(serverId, cancellationToken);
        var player = ServerManager.GetPlayer(server, initiatingPlayerConnectionId);
        var (wasRemoved, removedPlayer) = ServerManager.TryRemovePlayer(server, playerToRemovePublicId);

        if (wasRemoved && removedPlayer is not null)
        {
            RaisePlayerKicked(serverId, removedPlayer);
            RaiseRoomUpdated(serverId, server);
            RaiseLogUpdated(serverId, player.Username, $"Kicked {removedPlayer.Username}");
        }
    }
    public async Task SetPlayerAsleepInAllRoomsAsync(string playerConnectionId, CancellationToken cancellationToken = default)
    {
        await foreach (var server in _store.GetAllServersAsyncStream(cancellationToken)
                           .Where(s => s.Players.ContainsKey(playerConnectionId))
                           .WithCancellation(cancellationToken))
        {
            ServerManager.SetPlayerToSleep(server, playerConnectionId);

            RaiseRoomUpdated(server.Id, server);
        }
    }
    public async Task PlayCardAsync(Guid serverId, string playerConnectionId, WhiteCard card, CancellationToken cancellationToken = default)
    {
        var server = await _store.GetServerByIdAsync(serverId, cancellationToken);

        if (!server.CurrentSession.CardSet.WhiteCards.Contains(card))
        {
            throw new CannotPlayCardException($"Card does not exist in current card set");
        }


        var player = ServerManager.GetPlayer(server, playerConnectionId);

        player.Type
            .When(GameRoles.CardTsar, GameRoles.Observer)
            .Then(() => throw new CannotPlayCardException($"Player is of type '{player.Type}' and cannot play a card"));

        if (player.Mode == PlayerMode.Asleep)
        {
            throw new CannotPlayCardException($"Player is in mode '{player.Mode}', and cannot play a card");
        }

        SessionManager.PlayWhiteCard(server.CurrentSession, player.PublicId, card);

        RaiseRoomUpdated(server.Id, server);
        RaiseLogUpdated(server.Id, player.Username, "Played card.");
    }
    public async Task RedactWhiteCardAsync(Guid serverId, string playerConnectionId, CancellationToken cancellationToken = default)
    {
        var server = await _store.GetServerByIdAsync(serverId, cancellationToken);
        if (!server.CurrentSession.CanPlay)
        {
            throw new CannotRedactCardException($"Session not in state where players can remove their played cards");
        }

        if (!server.Players.ContainsKey(playerConnectionId))
        {
            throw new KeyNotFoundException("Player is not part of this session");
        }

        var player = ServerManager.GetPlayer(server,playerConnectionId);

        player.Type
            .When(GameRoles.CardTsar, GameRoles.Observer)
            .Then(() => throw new CannotPlayCardException($"Player is of type '{player.Type}' and cannot play a card"));

        if (player.Mode == PlayerMode.Asleep)
        {
            throw new CannotPlayCardException($"Player is in mode '{player.Mode}', and cannot play a card");
        }

        SessionManager.UnplayWhiteCard(server.CurrentSession, player.PublicId);
        RaiseRoomUpdated(server.Id, server);
        RaiseLogUpdated(server.Id, player.Username, "Redacted their played card");
    }
    public async Task ClearCardsAsync(Guid serverId, string playerConnectionId, CancellationToken cancellationToken = default)
    {
        var server = await _store.GetServerByIdAsync(serverId, cancellationToken);
        if (server.CurrentSession.CanClear)
        {
            throw new CannotClearBoardException($"Session not in state where cards can be cleared from the board");
        }

        SessionManager.ClearGameBoard(server.CurrentSession);

        var player = ServerManager.GetPlayer(server, playerConnectionId);
        RaiseRoomUpdated(server.Id, server);
        RaiseRoomCleared(server.Id);
        RaiseLogUpdated(server.Id, player.Username, "Cleared all the played cards");
    }
    public async Task ShowCardsAsync(Guid serverId, string playerConnectionId, CancellationToken cancellationToken = default)
    {
        var server = await _store.GetServerByIdAsync(serverId, cancellationToken);
        
        if (!server.CurrentSession.CanShow(server.Players))
        {
            throw new CannotShowCardsException($"Session not in state where played cards can be shown");
        }

        SessionManager.ShowCards(server.CurrentSession);
        var player = ServerManager.GetPlayer(server, playerConnectionId);
        RaiseRoomUpdated(server.Id, server);
        RaiseLogUpdated(server.Id, player.Username, "Made all played cards visible");
    }
    public async Task<Player> ChangePlayerTypeAsync(Guid serverId, string playerConnectionId, GameRoles newRole, CancellationToken cancellationToken = default)
    {
        var server = await _store.GetServerByIdAsync(serverId, cancellationToken);
        var player = ServerManager.GetPlayer(server, playerConnectionId);
        if (SessionManager.HasPlayedACard(server.CurrentSession, player.PublicId))
        {
            throw new CannotChangePlayerTypeException($"Cannot change from playertype '{GameRoles.Player}', when player has voted");
        }

        ServerManager.ChangePlayerType(player, newRole);
        RaiseRoomUpdated(server.Id, server);
        RaiseLogUpdated(server.Id, player.Username, $"Changed their player type {newRole}");
        return player;
    }
    public async Task<(bool wasCreated, Guid? serverId, string? validationmessages)> CreateServerAsync(CrowGameInputViewModel crowGameBase, CancellationToken cancellationToken = default)
    {
        var (isValidDeck, desiredCardDeck, validationMessage) = CardSetProcessor.TryParseCardSet(crowGameBase.DesiredPacks);

        if (!isValidDeck || desiredCardDeck is null)
        {
            return (false, null, validationMessage);
        }

        var createModel = new CreateCrowGameViewModel(crowGameBase.LobbyName, crowGameBase.LobbyCode, crowGameBase.Participants, desiredCardDeck);

        var createdServer = await _store.CreateServerAsync(createModel, cancellationToken);

        return (true, createdServer.Id, validationMessage);
    }
    public async Task<Player> JoinRoomAsync(Guid serverId, Guid recoveryId, string playerName, string playerConnectionId, GameRoles role, CancellationToken cancellationToken = default)
    {
        if (String.IsNullOrWhiteSpace(playerName))
        {
            throw new MissingPlayerNameException();
        }

        var server = await _store.GetServerByIdAsync(serverId, cancellationToken);

        var formattedPlayerName = playerName.Length > MaximumPlayerNameLength 
            ? playerName[..MaximumPlayerNameLength] 
            : playerName;

        var newPlayer = ServerManager.AddOrUpdatePlayer(server, recoveryId, playerConnectionId, formattedPlayerName, role);
        RaiseRoomUpdated(serverId, server);
        RaiseLogUpdated(serverId, newPlayer.Username, "Joined the server");
        return newPlayer;
    }
    #endregion
    #region Private Methods
    private void RaiseRoomCleared(Guid serverId) => RoomCleared?.Invoke(this, new RoomClearedEventArgs(serverId));
    private void RaiseLogUpdated(Guid serverId, string initiatingPlayerConnectionId, string logMessage) => LogUpdated?.Invoke(this, new LogUpdatedEventArgs(serverId, initiatingPlayerConnectionId, logMessage));
    private void RaiseRoomUpdated(Guid serverId, CrowGameServer updatedServer) => RoomUpdated?.Invoke(this, new RoomUpdatedEventArgs(serverId, updatedServer));
    private void RaisePlayerKicked(Guid serverId, Player removedPlayer) => PlayerKicked?.Invoke(this, new PlayerKickedEventArgs(serverId, removedPlayer));
    #endregion
}
