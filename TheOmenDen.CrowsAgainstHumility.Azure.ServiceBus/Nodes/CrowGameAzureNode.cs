using System;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Buses;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Extensions;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging.Messages;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Structures;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels.Game;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Locks;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Serializers;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Nodes;
public class CrowGameAzureNode : IDisposable, IAsyncDisposable
{
    #region Constants
    private const string DeletedLobbyPrefix = "Deleted:";
    private static readonly byte[] DeletedLobbyPrefixBytes = Encoding.UTF8.GetBytes(DeletedLobbyPrefix);
    #endregion
    #region Injected Members
    private readonly InitializationList _lobbiesToInitialize = new();
    private readonly CrowGameServerSerializer _gameServerSerializer;
    private readonly ILogger<CrowGameAzureNode> _logger;
    #endregion
    #region Disposable Members
    private IDisposable? _sendNodeMessageSubscription;
    private IDisposable? _serviceBusLobbyMessageSubscription;
    private IDisposable? _serviceBusLobbyCreatedMessageSubscription;
    private IDisposable? _serviceBusRequestLobbyListMessageSubscription;
    private IDisposable? _serviceBusRequestLobbiesMessageSubscription;
    private IDisposable? _serviceBusLobbyListMessageSubscription;
    private IDisposable? _serviceBusInitializeLobbyMessageSubscription;
    #endregion
    private volatile string? _processingLobbyCode;
    #region Constructors
    public CrowGameAzureNode(
            IAzureCrowGame crowGame,
            IServiceBus serviceBus,
            IAzureCrowGameConfiguration? configuration,
            CrowGameServerSerializer? gameServerSerializer,
            ILogger<CrowGameAzureNode> logger)
    {
        ArgumentNullException.ThrowIfNull(crowGame);
        ArgumentNullException.ThrowIfNull(serviceBus);
        ArgumentNullException.ThrowIfNull(logger);

        CrowGame = crowGame;
        ServiceBus = serviceBus;
        Configuration = configuration ?? new AzureCrowGameConfiguration();
        _logger = logger;
        _gameServerSerializer = gameServerSerializer ?? new(CrowGame.DateTimeProvider, CrowGame.GuidProvider);
        NodeId = CrowGame.GuidProvider.NewGuid().ToString();
    }
    #endregion
    #region Properties
    public IAzureCrowGame CrowGame { get; private set; }
    public string NodeId { get; private set; }
    public IAzureCrowGameConfiguration Configuration { get; private set; }
    protected IServiceBus ServiceBus { get; private set; }
    #endregion
    #region Lifecycle Methods
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.CrowGameAzureNodeStarting(NodeId);

        await ServiceBus.RegisterAsync(NodeId, cancellationToken);

        SetupCrowGameListeners();
        SetupServiceBusListeners();

        RequestLobbyList();
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.CrowGameAzureNodeStopping(NodeId);

        if (_sendNodeMessageSubscription is not null)
        {
            _sendNodeMessageSubscription.Dispose();
            _sendNodeMessageSubscription = null;
        }

        if (_serviceBusLobbyMessageSubscription is not null)
        {
            _serviceBusLobbyMessageSubscription.Dispose();
            _serviceBusLobbyMessageSubscription = null;
        }

        if (_serviceBusLobbyCreatedMessageSubscription is not null)
        {
            _serviceBusLobbyCreatedMessageSubscription.Dispose();
            _serviceBusLobbyCreatedMessageSubscription = null;
        }

        if (_serviceBusRequestLobbyListMessageSubscription is not null)
        {
            _serviceBusRequestLobbyListMessageSubscription.Dispose();
            _serviceBusRequestLobbyListMessageSubscription = null;
        }

        if (_serviceBusRequestLobbiesMessageSubscription is not null)
        {
            _serviceBusRequestLobbiesMessageSubscription.Dispose();
            _serviceBusRequestLobbiesMessageSubscription = null;
        }

        if (_serviceBusLobbyListMessageSubscription is not null)
        {
            _serviceBusLobbyListMessageSubscription.Dispose();
            _serviceBusLobbyListMessageSubscription = null;
        }

        if (_serviceBusInitializeLobbyMessageSubscription is not null)
        {
            _serviceBusInitializeLobbyMessageSubscription.Dispose();
            _serviceBusInitializeLobbyMessageSubscription = null;
        }

        return ServiceBus.UnregisterAsync(cancellationToken);
    }
    #endregion
    #region Disposal Methods
    ~CrowGameAzureNode()
    {
        Dispose(false);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopAsync().Wait();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }
    #endregion
    #region Private Static Methods
    private static byte[] CreateDeletedLobbyData(string lobbyCode) => Encoding.UTF8.GetBytes(DeletedLobbyPrefix + lobbyCode);

    private static string? ParseDeletedLobbyData(byte[]? lobbyData)
    {
        if (lobbyData is null || lobbyData.Length <= DeletedLobbyPrefixBytes.Length)
        {
            return null;
        }

        return DeletedLobbyPrefixBytes.Where((t, i) => lobbyData[i] != t).Any() 
            ? null 
            : Encoding.UTF8.GetString(lobbyData, DeletedLobbyPrefixBytes.Length, lobbyData.Length - DeletedLobbyPrefixBytes.Length);
    }
    #endregion
    #region Private Methods
    private Task SetupCrowGameListeners(CancellationToken cancellationToken)
    {
        var lobbyMessages = CrowGame.GetObservableMessages()
            .Where(m => !String.Equals(m.LobbyCode, _processingLobbyCode, StringComparison.OrdinalIgnoreCase));

        var nodeLobbyMessages = lobbyMessages
            .Where(m => m.MessageType != MessageTypes.Empty
                        && m.MessageType != MessageTypes.PlayerListCreated
                        && m.MessageType != MessageTypes.GameRoundEnded)
            .Select(m => new ObjectNodeMessage(NodeMessageTypes.PlayerListMessage, null, null, m));

        var createLobbyMessages = (IObservable<ObjectNodeMessage>)lobbyMessages
            .Where(m => m.MessageType == MessageTypes.PlayerListCreated)
            .Select(m => CreateLobbyCreatedMessage(m.LobbyCode))
            .Where(m => m is not null);

        var nodeMessages = nodeLobbyMessages.Merge(createLobbyMessages);

        _sendNodeMessageSubscription = nodeMessages.Subscribe(SendNodeMessage);
    }

    private Task SetupServiceBusListenersAsync(CancellationToken cancellationToken)
    {
        var serviceBusMessages = ServiceBus.GetObservableMessages()
            .Where(m =>
                !String.Equals(m.SenderNodeId, NodeId, StringComparison.OrdinalIgnoreCase));

        var busLobbyMessages = serviceBusMessages
            .Where(m => 
                m.MessageType == NodeMessageTypes.PlayerListMessage);
        _serviceBusLobbyMessageSubscription = busLobbyMessages.Subscribe(ProcessLobbyMessage);

        var busLobbyCreatedMessages = serviceBusMessages
            .Where(m => m.MessageType == NodeMessageTypes.LobbyCreated);
        
        _serviceBusLobbyCreatedMessageSubscription = busLobbyCreatedMessages.Subscribe(
            async m 
                => await OnLobbyCreatedAsync(m, cancellationToken)
                );
    }

    private async ValueTask<ObjectNodeMessage?> CreateLobbyCreatedMessage(string lobbyCode)
    {
        try
        {


            return new ObjectNodeMessage(NodeMessageTypes.LobbyCreated, null, null, new object());
        }
        catch(Exception ex)
        {
            _logger.ErrorCreateLobbyNodeMessage(ex, NodeId, lobbyCode);
            return null;
        }
    }

    private async Task SendNodeMessageAsync(ObjectNodeMessage message)
    {
        try
        {
            message = message with
            {
                SenderNodeId = NodeId
            };

            await ServiceBus.SendMessageAsync(message);

            _logger.NodeMessageSent(NodeId, message.SenderNodeId, message.RecipientNodeId, message.MessageType);
        }
        catch (Exception ex)
        {
            _logger.ErrorSendingNodeMessage(ex, NodeId, message.SenderNodeId, message.RecipientNodeId, message.MessageType);
        }
    }

    private async Task OnLobbyCreatedAsync(ObjectNodeMessage message, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lobby = await DeserializeServerAsync(message.DataToByteArray(), cancellationToken);

            _logger.LobbyCreatedNodeMessageReceived(NodeId, message.SenderNodeId, message.RecipientNodeId, message.MessageType, lobby.Code);

            if (!_lobbiesToInitialize.ContainsOrNotInitialized(lobby.Code))
            {
                _processingLobbyCode = lobby.Code;

                await CrowGame.InitializeLobbyAsync(lobby, cancellationToken);
            }

            _processingLobbyCode = null;
        }
        catch (Exception ex)
        {
            _logger.ErrorCreatingLobbyNodeMessage(ex, NodeId, message.SenderNodeId, message.RecipientNodeId, message.MessageType);
        }
    }

    private void ProcessLobbyMessage(ObjectNodeMessage nodeMessage, CancellationToken cancellationToken)
    {
        var message = (LobbyMessage)nodeMessage.Data!;
        _logger.LobbyNodeMessageReceived(NodeId,nodeMessage.SenderNodeId, nodeMessage.RecipientNodeId, nodeMessage.MessageType, message.LobbyCode, message.MessageType);

        try
        {
            if (!_lobbiesToInitialize.ContainsOrNotInitialized(message.LobbyCode))
            {
                message.MessageType
                    .When(MessageTypes.PlayerJoined)
                        .Then(() => OnPlayerJoinedMessage(message.LobbyCode, (LobbyMemberMessage)message))
                    .When(MessageTypes.PlayerDisconnected)
                        .Then(() => OnPlayerDisconnectedMessage(message.LobbyCode, (LobbyMemberMessage)message))
                    .When(MessageTypes.GameRoundStarted)
                        .Then(() => OnRoundStartedMessage(message.LobbyCode))
                    .When(MessageTypes.GameRoundCanceled)
                        .Then(() => OnRoundCanceledMessage(message.LobbyCode))
                    .When(MessageTypes.PlayerPlayedACard)
                        .Then(() => OnPlayerPlayedACard(message.LobbyCode, (LobbyMemberWhiteCardMessage)message))
                    .When(MessageTypes.AvailableCardsChanged)
                        .Then(() => OnAvailableCardsChangedMessage(message.LobbyCode, (LobbyWhiteCardPlayedMessage)message))
                    .When(MessageTypes.TimerStarted)
                        .Then(() => OnTimerStartedMessage(message.LobbyCode, (LobbyRoundTimerMessage)message))
                    .When(MessageTypes.TimerCanceled)
                        .Then(() => OnTimerCanceledMessage(message.LobbyCode))
                    .When(MessageTypes.PlayerActivity)
                        .Then(() => OnPlayerActivityMessage(message.LobbyCode, (LobbyMemberMessage)message));
            }
        }
        catch (Exception ex)
        {
            _logger.ErrorProcessingLobbyNodeMessage(ex, NodeId, nodeMessage.SenderNodeId, nodeMessage.RecipientNodeId, nodeMessage.MessageType, message.LobbyCode, message.MessageType);
        }
    }
    #endregion
    #region Serialization Methods

    private byte[] SerializeServer(CrowGameServer server)
    {
        using var ms = new MemoryStream();
        _gameServerSerializer.SerializeIntoStream(ms, server);
        return ms.ToArray();
    }
    private async ValueTask<byte[]> SerializeServerAsync(CrowGameServer server, CancellationToken cancellationToken)
    {
        await using var ms = new MemoryStream();

        await _gameServerSerializer.SerializeIntoStreamAsync(ms, server, cancellationToken);

        return ms.ToArray();
    }

    private CrowGameServer DeserializeServer(byte[] json)
    {
        using var ms = new MemoryStream(json);
        return _gameServerSerializer.DeserializeFromStream(ms);
    }

    private async ValueTask<CrowGameServer> DeserializeServerAsync(byte[] json, CancellationToken cancellationToken)
    {
        await using var ms = new MemoryStream(json);

        var lobby = await _gameServerSerializer.DeserializeFromStreamAsync(ms, cancellationToken);

        return lobby;
    }
    #endregion
}
