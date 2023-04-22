using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Reactive.Subjects;
using TheOmenDen.CrowsAgainstHumility.Azure.Redis.Extensions;
using TheOmenDen.CrowsAgainstHumility.Azure.Redis.MessageConverters;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Redis.ServiceBus;
internal class RedisServiceBus : IServiceBus, IDisposable
{
    #region Constants
    private const string DefaultChannelName = "CrowsAgainstHumility";
    #endregion
    #region Private Members
    private readonly Subject<NodeMessage> _observableMessages = new();
    private readonly ILogger<RedisServiceBus> _logger;
    private volatile string? _nodeId;
    private string? _connectionString;
    private string? _channel;
    private RedisChannel _redisChannel;
    private ConnectionMultiplexer? _redis;
    private ISubscriber? _subscriber;
    private bool _isSubscribed;
    #endregion
    #region Constructors and Finializer
    public RedisServiceBus(IRedisMessageConverter messageConverter, IAzureCrowGameConfiguration configuration,
        ILogger<RedisServiceBus> logger)
    {
        MessageConverter = messageConverter;
        Configuration = configuration;
        _logger = logger;
    }

    ~RedisServiceBus()
    {
        Dispose(false);
    }
    #endregion
    #region Public Properties
    public IRedisMessageConverter MessageConverter { get; private set; }
    public IAzureCrowGameConfiguration Configuration { get; private set; }
    public IObservable<NodeMessage> ObservableMessages => _observableMessages;
    #endregion
    #region IServiceBus Implementations
    public async Task SendMessageAsync(NodeMessage message, CancellationToken cancellationToken = default)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var subscriber = _subscriber;

        if (subscriber is null)
        {
            throw new InvalidOperationException("Redis PubSub is not initialized.");
        }

        var redisMessage = MessageConverter.ConvertToRedisMessage(message);

        try
        {
            await subscriber.PublishAsync(_redisChannel, redisMessage, CommandFlags.FireAndForget);
            _logger.SendMessage();
        }
        catch (Exception ex)
        {
            _logger.ErrorSendMessage(ex);
        }
    }

    public async Task RegisterAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        if (String.IsNullOrEmpty(nodeId))
        {
            throw new ArgumentNullException(nameof(nodeId));
        }

        _connectionString = GetConnectionString();
        _nodeId = nodeId;
        _channel = Configuration.ServiceBusTopic;

        if (String.IsNullOrEmpty(_channel))
        {
            _channel = DefaultChannelName;
        }

        _redisChannel = new RedisChannel(_channel, RedisChannel.PatternMode.Literal);
        _redis = await ConnectionMultiplexer.ConnectAsync(_connectionString);
        _subscriber = _redis.GetSubscriber();
        await CreateSubscriptionAsync(_redisChannel, _channel, nodeId);
    }

    public async Task UnregisterAsync(CancellationToken cancellationToken = default)
    {
        if (_isSubscribed && _subscriber is not null)
        {
            await _subscriber.UnsubscribeAsync(_redisChannel, null, CommandFlags.FireAndForget);
            _subscriber = null;
        }

        if (!_observableMessages.IsDisposed)
        {
            _observableMessages.OnCompleted();
            _observableMessages.Dispose();
        }

        if (_redis is not null)
        {
            await _redis.CloseAsync();
            await _redis.DisposeAsync();
            _redis = null;
        }
    }
    #endregion
    #region Private Methods
    private async Task CreateSubscriptionAsync(RedisChannel redisChannel, string channelName, string nodeId)
    {
        if (_isSubscribed)
        {
            return;
        }

        await _subscriber!.SubscribeAsync(redisChannel, ReceiveMessage);
        _isSubscribed = true;
        _logger.SubscriptionCreated(channelName, nodeId);
    }

    private void ReceiveMessage(RedisChannel redisChannel, RedisValue redisValue)
    {
        var nodeId = _nodeId;

        if (!redisValue.HasValue || nodeId is null)
        {
            return;
        }

        var nodeMessage = MessageConverter.GetMessageHeader(redisValue);
        var messageId = $"{nodeMessage.SenderNodeId}${nodeMessage.RecipientNodeId}${nodeMessage.MessageType}";
        _logger.MessageReceived(_channel, nodeId, messageId);

        if (nodeMessage.SenderNodeId == nodeId
            || nodeMessage.RecipientNodeId is not null && nodeMessage.RecipientNodeId != nodeId)
        {
            return;
        }

        try
        {
            nodeMessage = MessageConverter.ConvertToNodeMessage(redisValue);
            _observableMessages.OnNext(nodeMessage);
            _logger.MessageProcessed(_channel, nodeId, messageId);
        }
        catch (Exception ex)
        {
            _logger.ErrorProcessMessage(ex, _channel, nodeId, messageId);
        }
    }

    private string GetConnectionString()
    {
        var connectionString = Configuration.ServiceBusConnectionString!;

        if (connectionString.StartsWith("REDIS:", StringComparison.Ordinal))
        {
            connectionString = connectionString.Substring(6);
        }

        return connectionString;
    }
    #endregion
    #region Dispose Methods
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnregisterAsync().Wait();
        }
    }
    #endregion
}
