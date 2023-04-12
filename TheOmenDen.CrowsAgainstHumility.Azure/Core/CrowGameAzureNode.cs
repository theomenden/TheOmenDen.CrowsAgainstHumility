using System.Reactive.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Azure.Configuration;
using TheOmenDen.CrowsAgainstHumility.Azure.Extensions;
using TheOmenDen.CrowsAgainstHumility.Azure.Interfaces;
using TheOmenDen.CrowsAgainstHumility.Azure.Messages;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Core.Serialization;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Core;
public class CrowGameAzureNode : IDisposable, IAsyncDisposable
{
    #region Constants
    private const string DeletedPlayerListPrefix = "Deleted:";
    private static readonly byte[] DeletedPlayerListPrefixBytes = Encoding.UTF8.GetBytes(DeletedPlayerListPrefix);
    #endregion
    #region Private Readonly Members
    private readonly InitializationList _playerListsToInitialize = new();
    private readonly PlayerListSerializer _playerListSerializer;
    private readonly ILogger<CrowGameAzureNode> _logger;
    #endregion
    #region Subscribable Disposal Members
    private IDisposable? _sendNodeMessageSubscription;
    private IDisposable? _serviceBusPlayerListMessageSubscription;
    private IDisposable? _serviceBusPlayerListCreatedMessageSubscription;
    private IDisposable? _serviceBusRequestPlayerListMessageSubscription;
    private IDisposable? _serviceBusRequestPlayerListsMessageSubscription;
    private IDisposable? _serviceBusPlayerListsMessageSubscription;
    private IDisposable? _serviceBusInitializePlayerListMessageSubscription;
    #endregion
    #region Volatile Members
    private volatile string? _processingPlayerListName;
    #endregion
    #region Public Properties
    public IAzureCrowGame CrowGame { get; private set; }
    public string NodeId { get; private set; }
    public IAzureCrowGameConfiguration Configuration { get; private set; }
    protected IServiceBus ServiceBus { get; private set; }
    #endregion
    #region Starting/Stopping Methods
    public async Task Start()
    {
        _logger.CrowGameAzureNodeStarting(NodeId);

        await ServiceBus.RegisterAsync(NodeId);
        SetupCrowGameListeners();
        SetupServiceBusListeners();

        RequestLobbyList();
    }

    public Task Stop()
    {
        _logger.CrowGameAzureNodeStopping(NodeId);

        if (_sendNodeMessageSubscription is not null)
        {
            _sendNodeMessageSubscription.Dispose();
            _sendNodeMessageSubscription = null;
        }

        if (_serviceBusPlayerListMessageSubscription is not null)
        {
            _serviceBusPlayerListMessageSubscription.Dispose();
            _serviceBusPlayerListMessageSubscription = null;
        }

        if (_serviceBusPlayerListCreatedMessageSubscription is not null)
        {
            _serviceBusPlayerListCreatedMessageSubscription.Dispose();
            _serviceBusPlayerListCreatedMessageSubscription = null;
        }

        if (_serviceBusRequestPlayerListMessageSubscription is not null)
        {
            _serviceBusRequestPlayerListMessageSubscription.Dispose();
            _serviceBusRequestPlayerListMessageSubscription = null;
        }

        if (_serviceBusRequestPlayerListsMessageSubscription is not null)
        {
            _serviceBusRequestPlayerListsMessageSubscription.Dispose();
            _serviceBusRequestPlayerListsMessageSubscription = null;
        }

        if (_serviceBusPlayerListMessageSubscription is not null)
        {
            _serviceBusPlayerListMessageSubscription.Dispose();
            _serviceBusPlayerListMessageSubscription = null;
        }

        if (_serviceBusInitializePlayerListMessageSubscription is not null)
        {
            _serviceBusInitializePlayerListMessageSubscription.Dispose();
            _serviceBusInitializePlayerListMessageSubscription = null;
        }

        return ServiceBus.UnregisterAsync();
    }
    #endregion
    #region Setup Methods
    private void SetupCrowGameListeners()
    {
        var playerListMessages = CrowGame.ObservableMessages.Where(m =>
            !String.Equals(m.PlayerListName, _processingPlayerListName, StringComparison.OrdinalIgnoreCase));

        var nodePlayerListMessages = playerListMessages
            .Where(m => m.MessageType != MessageTypes.Empty
                        && m.MessageType != MessageTypes.PlayerListCreated
                        && m.MessageType != MessageTypes.GameRoundEnded)
            .Select(m => new NodeMessage(NodeMessageTypes.PlayerListMessage) { Data = m });

        var createPlayerListMessages = (IObservable<NodeMessage>)playerListMessages
            .Where(m => m.MessageType == MessageTypes.PlayerListCreated)
            .Select(m => CreatePlayerListCreatedMessage(m.PlayerListName))
            .Where(m => m is not null);

        var nodeMessages = nodePlayerListMessages.Merge(createPlayerListMessages);

        _sendNodeMessageSubscription = nodeMessages.Subscribe(SendNodeMessage);
    }

    private void SetupServiceBusListeners()
    {
        var serviceBusMessages = ServiceBus.ObservableMessages.Where(m =>
            !String.Equals(m.SenderNodeId, NodeId, StringComparison.OrdinalIgnoreCase));

        var busPlayerListMessages = serviceBusMessages.Where(m => m.MessageType == NodeMessageTypes.PlayerListMessage);
        _serviceBusPlayerListMessageSubscription = busPlayerListMessages.Subscribe(ProcessRequestLobbyMessage);

        var busPlayerListCreatedMessages =
            serviceBusMessages.Where(m => m.MessageType == NodeMessageTypes.LobbyCreated);

        _serviceBusPlayerListCreatedMessageSubscription = busPlayerListCreatedMessages.Subscribe(OnPlayerListCreated);
    }

    private void EndInitialization()
    {
        _playerListsToInitialize.Clear();

        if (_serviceBusInitializePlayerListMessageSubscription is not null)
        {
            _serviceBusInitializePlayerListMessageSubscription.Dispose();
            _serviceBusInitializePlayerListMessageSubscription = null;
        }

        CrowGame.EndInitialization();
        _logger.CrowsAgainstHumilityAzureNodeInitialized(NodeId);

        var serviceBusMessages = ServiceBus.ObservableMessages.Where((m => !String.Equals(m.SenderNodeId, NodeId, StringComparison.OrdinalIgnoreCase)));

        var requestPlayerListMessages = serviceBusMessages.Where(m => m.MessageType == NodeMessageTypes.RequestPlayerList);
        _serviceBusRequestPlayerListMessageSubscription =
            requestPlayerListMessages.Subscribe(ProcessRequestLobbyMessage);

        var requestPlayerListsMessages =
            serviceBusMessages.Where(m => m.MessageType == NodeMessageTypes.RequestPlayers);
        _serviceBusRequestPlayerListsMessageSubscription = requestPlayerListsMessages.Subscribe(ProcessRequestLobbyListMessage);
    }
    #endregion
    #region Sending Node Messages
    private async Task SendNodeMessage(NodeMessage message)
    {
        try
        {
            message.SenderNodeId = NodeId;
            await ServiceBus.SendMessageAsync(message);

            _logger.NodeMessageSent(NodeId, message.SenderNodeId, message.RecipientNodeId, message.MessageType);
        }
        catch (Exception ex)
        {
            _logger.ErrorSendingNodeMessage(ex, NodeId, message.SenderNodeId, message.RecipientNodeId, message.MessageType);
        }
    }
    #endregion
    #region Crow Game Round Messages
    private void OnRoundStartedMessage(string playerListName)
    {
        using var playerListLock = CrowGame.GetPlayerList(playerListName);
        playerListLock.Lock();
        var playerList = playerListLock.PlayerList;
        try
        {
            _processingPlayerListName = playerList.Name;
            playerList.CardTsar?.StartRound();
        }
        finally
        {
            _processingPlayerListName = null;
        }
    }

    private void OnroundCanceledMessage(string playerListName)
    {
        using var playerListLock = CrowGame.GetPlayerList(playerListName);

        playerListLock.Lock();
        var playerList = playerListLock.PlayerList;
        try
        {
            _processingPlayerListName = playerList.Name;
            playerList.CardTsar?.CancelRound();
        }
        finally
        {
            _processingPlayerListName = null;
        }
    }

    private void OnPlayerPlayedAWhiteCardMessage(string playerListName, PlayerListMemberWhiteCardMessage message)
    {
        using var playerListLock = CrowGame.GetPlayerList(playerListName);

        playerListLock.Lock();
        var playerList = playerListLock.PlayerList;
        try
        {
            _processingPlayerListName = playerList.Name;

            if (playerList.FindPlayerOrObserver(message.PlayerListName) is Member member)
            {
                member.WhiteCard = message.WhiteCard;
            }
        }
        finally
        {
            _processingPlayerListName = null;
        }
    }

    private void OnAvailableWhiteCardsChangedMessage(string playerListName, PlayerListWhiteCardsSetMessage message)
    {
        using var playerListLock = CrowGame.GetPlayerList(playerListName);

        playerListLock.Lock();
        var playerList = playerListLock.PlayerList;

        try
        {
            _processingPlayerListName = playerList.Name;
            var newAvailableWhiteCards = message.WhiteCards.Select(c => new WhiteCard { CardText = c.CardText, Id = c.Id, Pack = c.Pack, PackId = c.PackId }).ToList();
            playerList.ChangeAvailableWhiteCards(newAvailableWhiteCards!);
        }
        finally
        {
            _processingPlayerListName = null;
        }
    }

    private void OnTimerStartedMessage(string playerListName, PlayerListTimerMessage message)
    {
        using var playerListLock = CrowGame.GetPlayerList(playerListName);

        playerListLock.Lock();

        var playerList = playerListLock.PlayerList;
        try
        {
            _processingPlayerListName = playerList.Name;
            var remainingTimerDuration = message.EndTime - playerList.DateTimeProvider.UtcNow;

            if (remainingTimerDuration > TimeSpan.Zero)
            {
                playerList.CardTsar?.StartTimer(remainingTimerDuration);
            }
        }
        finally
        {
            _processingPlayerListName = null;
        }
    }

    private void OnTimerCanceledMessage(string playerListName)
    {
        using var playerListLock = CrowGame.GetPlayerList(playerListName);
        playerListLock.Lock();
        var playerList = playerListLock.PlayerList;

        try
        {
            _processingPlayerListName = playerList.Name;
            playerList.CardTsar?.CancelTimer();
        }
        finally
        {
            _processingPlayerListName = null;
        }
    }
    #endregion
    #region Player Member Messages
    private void OnPlayerActivityMessage(string playerListName, PlayerListMemberMessage message)
    {
        using var playerListLock = CrowGame.GetPlayerList(playerListName);
        playerListLock.Lock();
        var playerList = playerListLock.PlayerList;

        try
        {
            _processingPlayerListName = playerList.Name;
            var observer = playerList.FindPlayerOrObserver(message.MemberName);

            if (observer is null)
            {
                return;
            }

            observer.SessionId = message.SessionId;
            observer.AcknowledgeMessages(message.SessionId, message.AcknowledgedMessageId);
            observer.UpdateActivity();
        }
        finally
        {
            _processingPlayerListName = null;
        }
    }

    private void OnPlayerJoinedMessage(string playerListName, PlayerListMemberMessage message)
    {
        using var playerListLock = CrowGame.GetPlayerList(playerListName);

        playerListLock.Lock();
        var playerList = playerListLock.PlayerList;

        try
        {
            _processingPlayerListName = playerList.Name;
            var isObserver = message.GameRole == GameRoles.Observer;
            var observer = playerList.Join(message.MemberName, isObserver);
            observer.SessionId = message.SessionId;
        }
        finally
        {
            _processingPlayerListName = null;
        }
    }

    private void OnPlayerDisconnectedMessage(string playerListName, PlayerListMemberMessage message)
    {
        using var playerListLock = CrowGame.GetPlayerList(playerListName);
        playerListLock.Lock();

        var playerList = playerListLock.PlayerList;

        try
        {
            _processingPlayerListName = playerList.Name;
            playerList.Disconnect(message.MemberName);
        }
        finally
        {
            _processingPlayerListName = null;
        }
    }
    #endregion
    #region Deleted PlayerList Methods
    private static byte[] CreateDeletedPlayerListData(string playerListName) => Encoding.UTF8.GetBytes(DeletedPlayerListPrefix + playerListName);

    private static string? ParseDeletedPlayerListData(byte[] data)
    {
        if (data.Length <= DeletedPlayerListPrefixBytes.Length)
        {
            return null;
        }

        return DeletedPlayerListPrefixBytes.Where((t, i) => data[i] != t).Any()
            ? null
            : Encoding.UTF8.GetString(data, DeletedPlayerListPrefixBytes.Length, data.Length - DeletedPlayerListPrefixBytes.Length);
    }
    #endregion
    #region Player List Message Processing
    private void RequestLobbyList()
    {
        if (!_playerListsToInitialize.IsEmpty)
        {
            var serviceBusMessages = ServiceBus.ObservableMessages.Where(m => !String.Equals(m.SenderNodeId, NodeId, StringComparison.OrdinalIgnoreCase));
            var lobbyListActions = serviceBusMessages.Where(m => m.MessageType == NodeMessageTypes.PlayerList).Take(1)
                .Timeout(Configuration.InitializationMessageTimeout, Observable.Return<NodeMessage?>(null))
                .Select(m => new Action(() => ProcessLobbyMessage(m)));

            _serviceBusPlayerListMessageSubscription = lobbyListActions.Subscribe(a => a());

            return;
        }

        EndInitialization();
    }
    private void RequestLobbyList(string recipientId)
    {
        var playerListsToInitialize = _playerListsToInitialize.Values!.ToArray();

        if (playerListsToInitialize.Length == 0)
        {
            EndInitialization();
            return;
        }

        var lockObject = new object();
        var serviceBusMessages = ServiceBus.ObservableMessages.Where(m =>
            !String.Equals(m.SenderNodeId, NodeId, StringComparison.OrdinalIgnoreCase));

        serviceBusMessages = serviceBusMessages.Synchronize(lockObject);

        var lastMessageTime = CrowGame.DateTimeProvider.UtcNow;

        var initializeTeamActions = serviceBusMessages.Where(m => m.MessageType == NodeMessageTypes.InitializePlayers)
            .TakeWhile(m => !_playerListsToInitialize.IsEmpty)
            .Select(m => new Action(() =>
            {
                lastMessageTime = CrowGame.DateTimeProvider.UtcNow;
                ProcessInitializeLobbyMessage(m);
            }));

        var messageTimeoutActions = Observable.Interval(TimeSpan.FromSeconds(1d)).Synchronize(lockObject)
            .SelectMany(i => (lastMessageTime + Configuration.InitializationMessageTimeout) < CrowGame.DateTimeProvider.UtcNow
            ? Observable.Throw<Action>(new TimeoutException())
            : Observable.Empty<Action>());

        _serviceBusInitializePlayerListMessageSubscription = initializeTeamActions.Merge(messageTimeoutActions)
            .Subscribe(a => a(), RetryRequestLobbyList);

        var requestPlayerListsMessage = new NodeMessage(NodeMessageTypes.RequestPlayerList)
        {
            RecipientNodeId = recipientId,
            Data = playerListsToInitialize
        };

        SendNodeMessage(requestPlayerListsMessage);
    }

    private void ProcessInitializeLobbyMessage(NodeMessage message)
    {
        var playerListData = (byte[])message.Data!;

        var deletedPlayerListName = ParseDeletedPlayerListData(playerListData);

        if (!String.IsNullOrWhiteSpace(deletedPlayerListName))
        {
            _playerListsToInitialize.Remove(deletedPlayerListName);

            if (_playerListsToInitialize.IsEmpty)
            {
                EndInitialization();
            }

            return;
        }

        var playerList = DeserializePlayerList(playerListData);
        _logger.PlayerListCreatedNodeMessageReceived(NodeId, message.SenderNodeId, message.RecipientNodeId,
            message.MessageType, playerList.Name);

        _playerListsToInitialize.Remove(playerList.Name);
        CrowGame.InitializePlayerList(playerList);

        if (_playerListsToInitialize.IsEmpty)
        {
            EndInitialization();
        }
    }

    private void RetryRequestLobbyList(Exception exception)
    {
        if (_playerListsToInitialize.IsEmpty)
        {
            return;
        }

        _logger.RetryRequestPlayerList(NodeId);
        RequestLobbyList();
    }

    private void RequestLobby()
    {
        if (_playerListsToInitialize.IsEmpty)
        {
            EndInitialization();
            return;
        }

        var serviceBusMessage = ServiceBus.ObservableMessages.Where(m =>
            !String.Equals(m.SenderNodeId, NodeId, StringComparison.OrdinalIgnoreCase));

        var playerListActions = serviceBusMessage.Where(m => m.MessageType == NodeMessageTypes.PlayerList)
            .Take(1)
            .Timeout(Configuration.InitializationMessageTimeout, Observable.Return<NodeMessage?>(null))
            .Select(m => new Action(() => ProcessLobbyMessage(m)));

        _serviceBusPlayerListMessageSubscription = playerListActions.Subscribe(a => a());

        SendNodeMessage(new NodeMessage(NodeMessageTypes.RequestPlayerList));
    }

    private async Task RequestLobbyAsync()
    {
        if (_playerListsToInitialize.IsEmpty)
        {
            EndInitialization();
            return;
        }

        var serviceBusMessage = ServiceBus.ObservableMessages.Where(m =>
            !String.Equals(m.SenderNodeId, NodeId, StringComparison.OrdinalIgnoreCase));

        var playerListActions = serviceBusMessage.Where(m => m.MessageType == NodeMessageTypes.PlayerList)
            .Take(1)
            .Timeout(Configuration.InitializationMessageTimeout, Observable.Return<NodeMessage?>(null))
            .Select(m => new Action(() => ProcessLobbyMessage(m)));

        _serviceBusPlayerListMessageSubscription = playerListActions.Subscribe(a => a());

        await SendNodeMessage(new NodeMessage(NodeMessageTypes.RequestPlayerList));
    }

    private void ProcessLobbyMessage(NodeMessage? message)
    {
        if (_serviceBusPlayerListMessageSubscription is not null)
        {
            _serviceBusPlayerListMessageSubscription.Dispose();
            _serviceBusPlayerListMessageSubscription = null;
        }

        if (message is not null)
        {
            _logger.NodeMessageReceived(NodeId, message.SenderNodeId, message.RecipientNodeId, message.MessageType);

            var playerLists = ((IEnumerable<string>?)message?.Data ?? Enumerable.Empty<string>()).ToList();

            if (_playerListsToInitialize.Setup(playerLists))
            {
                CrowGame.SetInitializingPlayerLists(playerLists);
            }

            RequestLobbyList(message.SenderNodeId!);
            return;
        }

        EndInitialization();
    }

    private void ProcessRequestLobbyMessage(NodeMessage message)
    {
        _logger.NodeMessageReceived(NodeId, message.SenderNodeId, message.RecipientNodeId, message.MessageType);

        var playerListMessage = new NodeMessage(NodeMessageTypes.PlayerList)
        {
            RecipientNodeId = message.SenderNodeId,
            Data = CrowGame.PlayerListNames.ToArray()
        };

        SendNodeMessage(playerListMessage);
    }

    private void ProcessRequestLobbyListMessage(NodeMessage message)
    {
        _logger.NodeMessageReceived(NodeId, message.SenderNodeId, message.RecipientNodeId, message.MessageType);

        var playerListNames = (IEnumerable<string>)message.Data!;

        foreach (var playerList in playerListNames)
        {
            try
            {
                ProcessLobby(message, playerList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Sending Node Message: {@Ex}", ex);
            }
        }
    }

    private void ProcessLobby(NodeMessage message, string playerList)
    {
        byte[] playerListData;
        try
        {
            using var playerListLock = CrowGame.GetPlayerList(playerList);
            playerListLock.Lock();
            playerListData = SerializePlayerList(playerListLock.PlayerList);
        }
        catch (Exception ex)
        {
            playerListData = Array.Empty<byte>();
        }

        var initializePlayerListMessage = new NodeMessage(NodeMessageTypes.InitializePlayers)
        {
            RecipientNodeId = message.SenderNodeId,
            Data = playerListData.Length > 0 ? playerListData : CreateDeletedPlayerListData(playerList)
        };
        SendNodeMessage(initializePlayerListMessage);
    }
    #endregion
    #region Serialization of Player Lists
    private byte[] SerializePlayerList(PlayerList playerList)
    {
        using var stream = new MemoryStream();
        _playerListSerializer.Serialize(stream, playerList);
        return stream.ToArray();
    }

    private PlayerList DeserializePlayerList(byte[] jsonBytes)
    {
        using var stream = new MemoryStream(jsonBytes);
        return _playerListSerializer.Deserialize(stream);
    }
    #endregion
    #region Disposal Methods
    ~CrowGameAzureNode() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await Stop().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        Stop().Wait();
    }
    #endregion
}
