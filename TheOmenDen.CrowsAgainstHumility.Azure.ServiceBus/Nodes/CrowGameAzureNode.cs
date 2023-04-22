using System.Text;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Extensions;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Structures;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Nodes;
internal class CrowGameAzureNode : IDisposable
{
    #region Constants
    private const string DeletedLobbyPrefix = "Deleted:";
    private static readonly byte[] DeletedLobbyPrefixBytes = Encoding.UTF8.GetBytes(DeletedLobbyPrefix);
    #endregion
    #region Members
    private readonly InitializationList _lobbiesToInitialize = new();
    private readonly LobbySerializer _lobbySerializer;
    private readonly ILogger<CrowGameAzureNode> _logger;
    #endregion
    #region Disposable Subscriptions
    private IDisposable? _sendNodeMessageSubscription;
    private IDisposable? _serviceBusLobbyMessageSubscription;
    private IDisposable? _serviceBusLobbyCreatedMessageSubscription;
    private IDisposable? _serviceBusRequestLobbyListMessageSubscription;
    private IDisposable? _serviceBusRequestLobbiesMessageSubscription;
    private IDisposable? _serviceBusLobbyListMessageSubscription;
    private IDisposable? _serviceBusInitializeLobbyMessageSubscription;
    #endregion
    #region Constructors
    public CrowGameAzureNode(LobbySerializer lobbySerializer, IAzureCrowGame crowGame, IServiceBus serviceBus, IAzureCrowGameConfiguration? configuration, LobbySerializer? lobbySerializer, ILogger<CrowGameAzureNode> logger)
    {
        ArgumentNullException.ThrowIfNull(crowGame);
        ArgumentNullException.ThrowIfNull(serviceBus);
        ArgumentNullException.ThrowIfNull(logger);
        CrowGame = crowGame;
        ServiceBus = serviceBus;
        _lobbySerializer = lobbySerializer ?? new LobbySerializer(CrowGame.DateTimeProvider, CrowGame.GuidProvider);
        _lobbySerializer = lobbySerializer;
        _logger = logger;
        NodeId = CrowGame.GuidProvider.NewGuid().ToString();
    }
    #endregion
    #region Properties
    public IAzureCrowGame CrowGame { get; private set; }
    public string NodeId { get; private set; }
    public IAzureCrowGameConfiguration Configuration { get; private set; }
    protected IServiceBus ServiceBus { get; private set; }
    #endregion
    #region Public Methods
    public async Task Start(CancellationToken cancellationToken = default)
    {
        _logger.CrowGameAzureNodeStarting(NodeId);

        await ServiceBus.RegisterAsync(NodeId, cancellationToken);
        SetupCrowGameListeners();
        SetupServiceBusListeners();

        RequestLobbyList();
    }

    public Task Stop(CancellationToken cancellationToken = default)
    {
        _logger.CrowGameAzureNodeStopping(NodeId);

        CancelSubscriptions();

        return ServiceBus.UnregisterAsync(cancellationToken);
    }
    #endregion
    #region Disposal Methods

    ~CrowGameAzureNode() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Stop().Wait();
        }
    }

    private void CancelSubscriptions()
    {
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

        if (_serviceBusLobbyListMessageSubscription is not null)
        {
            _serviceBusLobbyListMessageSubscription.Dispose();
            _serviceBusLobbyListMessageSubscription = null;
        }

        if (_serviceBusRequestLobbiesMessageSubscription is not null)
        {
            _serviceBusRequestLobbiesMessageSubscription.Dispose();
            _serviceBusRequestLobbiesMessageSubscription = null;
        }

        if (_serviceBusRequestLobbyListMessageSubscription is not null)
        {
            _serviceBusRequestLobbyListMessageSubscription.Dispose();
            _serviceBusRequestLobbyListMessageSubscription = null;
        }

        if (_serviceBusInitializeLobbyMessageSubscription is not null)
        {
            _serviceBusInitializeLobbyMessageSubscription.Dispose();
            _serviceBusInitializeLobbyMessageSubscription = null;
        }
    }
    #endregion
    #region Private Static Methods
    private static byte[] CreateDeletedLobbyData(string lobbyName) => Encoding.UTF8.GetBytes(DeletedLobbyPrefix + lobbyName);

    private static string? ParseDeletedLobbyData(byte[] data)
    {
        if (data.Length <= DeletedLobbyPrefixBytes.Length)
        {
            return null;
        }

        return DeletedLobbyPrefixBytes.Where((t, i) => data[i] != t).Any()
            ? null
            : Encoding.UTF8.GetString(data, DeletedLobbyPrefixBytes.Length, data.Length - DeletedLobbyPrefixBytes.Length);
    }
    #endregion
    #region Private Methods
    private void SetupCrowGameListeners()
    {
        var lobbyMessages = CrowGame.ObservableMessages.Where(m => !String.Equals(m.LobbyName, _processingLobbyName, StringComparison.OrdinalIgnoreCase));

        var nodeLobbyMessages = lobbyMessages
            .Where(m => m.MessageType != MessageTypes.Empty
                        && m.MessageType != MessageTypes.PlayerListCreated
                        && m.Message != MessageTypes.GameRoundEnded)
            .Select(m => new NodeMessageTypes(NodeMessageTypes.LobbyMessage, m));

        var createdLobbyMessages = (IObservable<NodeMessage>)lobbyMessages
            .Where(m => m.MessageType == MessageTypes.PlayerListCreated)
            .Select(m => m.CreateLobbyCreatedMessage(m.LobbyName))
            .Where(m => m is not null);

        var nodeMessages = nodeLobbyMessages.Merge(createdLobbyMessages);

        _sendNodeMessageSubscription = nodeMessages.Subscribe(SendNodeMessage);
    }

    private void SetupServiceBusListeners()
    {
        var serviceBusMessages = ServiceBus.ObservableMessages.Where(m => !String.Equals(m.SenderNodeId, NodeId, StringComparison.OrdinalIgnoreCase));

        var busLobbyMessages = serviceBusMessages.Where(m => m.MessageType == NodeMessageTypes.PlayerListMessage);
        _serviceBusLobbyMessageSubscription = busLobbyMessages.Subscribe(ProcessLobbyMessage);

        var busLobbyCreatedMessages = serviceBusMessages.Where(m => m.Message == MessageTypes.PlayerListCreated);
        _serviceBusLobbyCreatedMessageSubscription = busLobbyCreatedMessages.Subscribe(OnLobbyCreated);
    }

    private NodeMessage CreateLobbyCreatedMessage(string lobbyCode)
    {
        try
        {
            using var lobbyLock = ServerManager.GetLobby(lobbyCode);

            lobbyLock.Lock();
            var lobby = lobbyLock.GetLobby();

            return new NodeMessage(NodeMessageTypes.LobbyCreated, SerializeLobby(lobby));
        }
        catch (Exception ex)
        {
            _logger.ErrorCreateLobbyNodeMessage(ex, NodeId, lobby);
            return null;
        }
    }
    #endregion
    #region Serialization Methods
    private byte[] SerializeLobby(Lobby lobby)
    {
        using var stream = new MemoryStream();

        _lobbySerializer.Serialize(stream, lobby);
        return stream.ToArray();
    }

    private Lobby DeserializeLobby(byte[] json)
    {
        using var stream = new MemoryStream(json);

        return _lobbySerializer.Deserialize(stream);
    }
    #endregion
}
