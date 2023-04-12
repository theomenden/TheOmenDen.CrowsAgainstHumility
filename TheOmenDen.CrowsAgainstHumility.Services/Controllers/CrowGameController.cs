using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Locks;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;
using TheOmenDen.CrowsAgainstHumility.Services.Decks;
using TheOmenDen.CrowsAgainstHumility.Services.Extensions;
using TheOmenDen.CrowsAgainstHumility.Services.Locks;
using TheOmenDen.CrowsAgainstHumility.Services.Messages;
using TheOmenDen.CrowsAgainstHumility.Services.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Tasks;
using TheOmenDen.Shared.Extensions;
using TheOmenDen.Shared.Guards;
using DateTimeProvider = TheOmenDen.CrowsAgainstHumility.Services.Providers.DateTimeProvider;
using Message = TheOmenDen.CrowsAgainstHumility.Core.Messages.Message;

namespace TheOmenDen.CrowsAgainstHumility.Services.Controllers;
internal class CrowGameController
{
    private readonly ConcurrentDictionary<string, Tuple<PlayerList, object>> _lobbies = new();
    private readonly PlayDeckService _playDeckService;
    private readonly TaskProvider _taskProvider;
    private readonly ILogger<CrowGameController> _logger;

    public CrowGameController()
    {

    }

    public DateTimeProvider DateTimeProvider { get; private set; }
    public GuidProvider GuidProvider { get; private set; }
    public ICrowGameConfiguration Configuration { get; private set; }

    protected IPlayerListRepository Repository { get; private set; }

    public IEnumerable<string> LobbyNames => _lobbies.ToArray().Select(p => p.Key)
                .Union(Repository.LobbyNames.ToArray(), StringComparer.OrdinalIgnoreCase)
                .ToArray();

    #region Public Methods
    public IPlayerListLock CreateLobby(string lobbyName, string initialCardTsar, Deck deck)
    {
        Guard.FromNullOrWhitespace(lobbyName, nameof(lobbyName));
        Guard.FromNullOrWhitespace(initialCardTsar, nameof(initialCardTsar));

        OnBeforeLobbyCreated(lobbyName, initialCardTsar);

        var availableWhiteCards = deck.WhiteCards.GetRandomElements(deck.WhiteCards.Count()).ToArray();
        var playerList = new PlayerList(lobbyName, availableWhiteCards, DateTimeProvider, GuidProvider);
        playerList.SetCardTsar(initialCardTsar);

        var lobbyLock = new object();
        var lobbyTuple = new Tuple<PlayerList, object>(playerList, lobbyLock);

        LoadLobby(lobbyName);

        if (!_lobbies.TryAdd(lobbyName, lobbyTuple))
        {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.PlayerListAlreadyExists, lobbyName), nameof(lobbyName));
        }

        OnLobbyAdded(playerList);
        _logger.LobbyCreated(lobbyTuple.Item1.Name, lobbyTuple.Item1.CardTsar!.Name);

        return new PlayerListLock(lobbyTuple.Item1, lobbyTuple.Item2);
    }

    public IPlayerListLock AttachLobby(PlayerList lobby)
    {
        if (lobby is null)
        {
            throw new ArgumentNullException(nameof(lobby));
        }

        var lobbyName = lobby.Name;
        var lobbyLock = new object();
        var lobbyTuple = new Tuple<PlayerList, object>(lobby, lobbyLock);

        LoadLobby(lobbyName);

        if (!_lobbies.TryAdd(lobbyName, lobbyTuple))
        {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.PlayerListAlreadyExists, lobbyName), nameof(lobby));
        }

        OnLobbyAdded(lobby);

        _logger.LobbyAttached(lobby.Name);

        return new PlayerListLock(lobbyTuple.Item1, lobbyTuple.Item2);
    }

    public IPlayerListLock GetPlayerList(string lobbyName)
    {
        Guard.FromNullOrWhitespace(lobbyName, nameof(lobbyName));

        OnBeforeGetLobby(lobbyName);

        var lobbyTuple = LoadLobby(lobbyName) ?? throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.PlayListDoesNotExist, lobbyName), nameof(lobbyName));

        _logger.ReadLobby(lobbyTuple.Item1.Name);

        return new PlayerListLock(lobbyTuple.Item1, lobbyTuple.Item2);
    }

    public Task<IEnumerable<Message>> GetMessagesAsync(Observer observer, CancellationToken cancellationToken = default)
    {
        Guard.FromNull(observer, nameof(observer));

        if (observer.HasMessage)
        {
            _logger.ObserverMessageReceived(observer.Name, observer.Players.Name, true);
            IEnumerable<Message> messages = observer.Messages.ToList();
            return Task.FromResult(messages);
        }

        if (!_lobbies.TryGetValue(observer.Players.Name, out var lobbyTuple) 
            || lobbyTuple?.Item1 != observer.Players)
        {
            throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, Resources.PlayListDoesNotExist, observer.Players.Name));
        }

        var lobbyLock = new PlayerListLock(lobbyTuple.Item1, lobbyTuple.Item2);
        var receiveMessagesTask = new ReceiveMessagesTask(lobbyLock, observer, _taskProvider);
        return receiveMessagesTask.GetMessagesAsync(Configuration.WaitForMessageTimeout, cancellationToken);
    }

    public void DisconnectInactiveObservers() => DisconnectInactiveObservers(Configuration.ClientInactivityTimeout);

    public void DisconnectInactiveObservers(TimeSpan inactivityTime)
    {
        var lobbyTuples = _lobbies.ToArray();

        foreach (var lobbyTuple in lobbyTuples)
        {
            using var lobbyLock = new PlayerListLock(lobbyTuple.Value.Item1, lobbyTuple.Value.Item2);

            lobbyLock.Lock();
            _logger.DisconnectingInactiveObservers(lobbyLock.Players.Name);
            lobbyLock.Players.DisconnectInactiveObservers(inactivityTime);
        }
    }
    #endregion
    protected virtual void OnLobbyAdded(PlayerList lobby)
    {
        Guard.FromNull(lobby, nameof(lobby));

        lobby.MessageReceived += new EventHandler<MessageReceivedEventArgs>(OnLobbyMessageReceived);
        _logger.DebugLobbyAdded(lobby.Name);
    }

    protected virtual void OnLobbyRemoved(PlayerList lobby)
    {
        Guard.FromNull(lobby, nameof(lobby));

        lobby.MessageReceived -= new EventHandler<MessageReceivedEventArgs>(OnLobbyMessageReceived);

        _logger.DebugLobbyRemoved(lobby.Name);
    }

    protected virtual void OnBeforeLobbyCreated(string lobbyName, string initialCardTsarName)
    {
        //Empty by default
    }

    protected virtual void OnBeforeGetLobby(string lobbyName)
    {
        // Empty by default
    }

    private static bool IsLobbyActive(PlayerList lobby) => lobby.Players.Any(m => !m.IsDormant) || lobby.Observers.Any(o => !o.IsDormant);

    private void OnLobbyMessageReceived(object? sender, MessageReceivedEventArgs e)
    {
        var lobby = (PlayerList)(sender ?? throw new ArgumentNullException(nameof(sender)));
        var shouldSaveLobby = true;

        LogLobbyMessage(lobby, e.Message);

        if (e.Message.MessageType == MessageTypes.PlayerDisconnected
            && !IsLobbyActive(lobby))
        {
            shouldSaveLobby = false;
            OnLobbyRemoved(lobby);

            _lobbies.TryRemove(lobby.Name, out _);
            Repository.DeletePlayerList(lobby.Name);
            _logger.LobbyRemoved(lobby.Name);
        }

        if (shouldSaveLobby)
        {
            SaveLobby(lobby);
        }
    }

    private Tuple<PlayerList, object>? LoadLobby(string lobbyName)
    {
        Tuple<PlayerList, object>? result = null;
        var shouldRetry = true;

        while (shouldRetry)
        {
            shouldRetry = false;

            if (!_lobbies.TryGetValue(lobbyName, out result))
            {
                var lobby = Repository.LoadPlayerList(lobbyName);

                if (lobby is not null
                    && VerifyLobbyActive(lobby))
                {
                    var lobbyLockObject = new object();
                    result = new Tuple<PlayerList, object>(lobby, lobbyLockObject);

                    if (_lobbies.TryAdd(lobby.Name, result))
                    {
                        OnLobbyAdded(lobby);
                        continue;
                    }

                    shouldRetry = true;
                }

                continue;
            }

            Repository.DeletePlayerList(lobbyName);
        }

        return result;
    }

    private void SaveLobby(PlayerList lobby) => Repository.SavePlayerList(lobby);

    private bool VerifyLobbyActive(PlayerList lobby)
    {
        lobby.DisconnectInactiveObservers(Configuration.ClientInactivityTimeout);
        return IsLobbyActive(lobby);
    }

    private void LogLobbyMessage(PlayerList lobby, Message message)
    {
        if (message is MemberMessage memberMessage)
        {
            _logger.MemberMessaged(lobby.Name, memberMessage.Id, memberMessage.Type, memberMessage.Member?.Name);
            return;
        }

        _logger.LobbyMessaged(lobby.Name, message.Id, message.MessageType);
    }
}
