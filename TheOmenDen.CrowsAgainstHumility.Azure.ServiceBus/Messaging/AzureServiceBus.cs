using System.Collections.Concurrent;
using System.Globalization;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Timers;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Buses;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Extensions;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Nodes;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging;
internal class AzureServiceBus : IServiceBus, IDisposable, IAsyncDisposable
{
    #region Constants
    private const string DefaultTopicName = "CrowGame";
    private const string SubscriptionPingPropertyname = "SubscriptionPing";
    private static readonly TimeSpan _serviceBusTokenTimeout = TimeSpan.FromMinutes(1);
    #endregion
    #region Injected Members
    private readonly ILogger<AzureServiceBus> _logger;
    private readonly ConcurrentDictionary<string, DateTime> _nodes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Subject<ObjectNodeMessage> _observableMessages = new();
    #endregion
    #region Fields
    private volatile string? _nodeId;
    private string? _connectionString;
    private string? _topicName;
    private ServiceBusAdministrationClient? _serviceBusAdministrationClient;
    private ServiceBusClient? _serviceBusClient;
    private ServiceBusSender? _serviceBusSender;
    private ServiceBusProcessor? _serviceBusProcessor;
    private System.Timers.Timer? _subscriptionMaintenanceTimer;
    #endregion
    #region Constructors
    public AzureServiceBus(IMessageConverter messageConverter, IAzureCrowGameConfiguration configuration, ILogger<AzureServiceBus> logger)
    {
        ArgumentNullException.ThrowIfNull(messageConverter);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        MessageConverter = messageConverter;
        Configuration = configuration;
        _logger = logger;
    }
    #endregion
    #region Properties
    public IMessageConverter MessageConverter { get; private set; }
    public IAzureCrowGameConfiguration Configuration { get; private set; }
    #endregion
    #region IServiceBus Implementations
    public IObservable<ObjectNodeMessage> GetObservableMessages()
    => _observableMessages is null
            ? Observable.Empty<ObjectNodeMessage>()
            : _observableMessages;

    public async Task SendMessageAsync(ObjectNodeMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var serviceBusSender = _serviceBusSender;

        if (serviceBusSender is null)
        {
            throw new InvalidOperationException("AzureServiceBus not initialized");
        }

        var serviceBusMessage = await MessageConverter.ConvertToServiceBusMessageAsync(message, cancellationToken);

        try
        {
            await serviceBusSender.SendMessageAsync(serviceBusMessage, cancellationToken);
            _logger.SendMessage();
        }
        catch (Exception ex)
        {
            _logger.SendErrorMessage(ex);
        }
    }

    public async Task RegisterAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(nodeId);

        _nodeId = nodeId;
        _connectionString = Configuration.ServiceBusConnectionString;
        _topicName = Configuration.ServiceBusTopic;

        if (String.IsNullOrEmpty(_topicName))
        {
            _topicName = DefaultTopicName;
        }

        _serviceBusAdministrationClient = new ServiceBusAdministrationClient(_connectionString);
        _serviceBusClient = new ServiceBusClient(_connectionString);
        _serviceBusSender = _serviceBusClient.CreateSender(_topicName);

        await CreateSubscriptionAsync(_topicName, _nodeId, cancellationToken);

        await SendSubscriptionAliveMessageAsync(cancellationToken);

        _subscriptionMaintenanceTimer = new System.Timers.Timer(Configuration.SubscriptionMaintenanceInterval.TotalMilliseconds);
        _subscriptionMaintenanceTimer.Elapsed += SubscriptionsMaintenanceTimerOnElapsed;
        _subscriptionMaintenanceTimer.Start();
    }

    public async Task UnregisterAsync(CancellationToken cancellationToken = default)
    {
        if (_subscriptionMaintenanceTimer is not null)
        {
            _subscriptionMaintenanceTimer.Dispose();
            _subscriptionMaintenanceTimer = null;
        }

        if (_serviceBusProcessor is not null)
        {
            await _serviceBusProcessor.DisposeAsync();
            _serviceBusProcessor = null;
        }

        if (!_observableMessages.IsDisposed)
        {
            _observableMessages.OnCompleted();
            _observableMessages.Dispose();
        }

        if (_serviceBusSender is not null)
        {
            await _serviceBusSender.DisposeAsync();
            _serviceBusSender = null;
        }

        if (_serviceBusClient is not null)
        {
            await _serviceBusClient.DisposeAsync();
            _serviceBusClient = null;
        }

        if (_serviceBusAdministrationClient is not null)
        {
            await DeleteSubscriptionAsync(cancellationToken);
            _serviceBusAdministrationClient = null;
        }
    }
    #endregion
    #region Private Methods

    private async void SubscriptionsMaintenanceTimerOnElapsed(object? sender, ElapsedEventArgs args)
    {
        try
        {
            await SendSubscriptionAliveMessageAsync(CancellationToken.None);
            await DeleteInactiveSubscriptionsAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.ErrorSubscriptionsMaintenance(ex, _nodeId);
        }
    }
    private IAsyncEnumerable<string> GetTopicSubscriptionsAsync(CancellationToken cancellationToken) => _serviceBusAdministrationClient is null
            ? AsyncEnumerable.Empty<string>()
            : _serviceBusAdministrationClient
            .GetSubscriptionsAsync(_topicName, cancellationToken)
            .Select(s => s.SubscriptionName);

    private async Task DeleteSubscriptionAsync(string name, CancellationToken cancellationToken)
    {
        if (_serviceBusAdministrationClient is null)
        {
            return;
        }

        try
        {
            var subscriptionExists = await _serviceBusAdministrationClient.SubscriptionExistsAsync(_topicName, name, cancellationToken);

            if (subscriptionExists)
            {
                await _serviceBusAdministrationClient.DeleteSubscriptionAsync(_topicName, name, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.SubscriptionDeleteFailed(ex, _topicName, name);
        }
    }

    private async Task DeleteInactiveSubscriptionsAsync(CancellationToken cancellationToken)
    {
        await foreach (var subscription in GetTopicSubscriptionsAsync(cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            if (String.Equals(subscription, _nodeId, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            _nodes.TryAdd(subscription, DateTime.UtcNow);

            if (!_nodes.TryGetValue(subscription, out var lastActiveAt))
            {
                continue;
            }

            if (lastActiveAt >= DateTime.UtcNow - Configuration.SubscriptionInactivityTimeout)
            {
                continue;
            }

            await DeleteSubscriptionAsync(subscription, cancellationToken);
            _nodes.TryRemove(subscription, out lastActiveAt);
            _logger.InactiveSubscriptionDeleted(_nodeId, subscription);
        }
    }

    private async Task SendSubscriptionAliveMessageAsync(CancellationToken cancellationToken)
    {
        if (_serviceBusSender is null)
        {
            return;
        }

        var message = new ServiceBusMessage
        {
            ApplicationProperties =
            {
                [SubscriptionPingPropertyname] = DateTime.UtcNow,
                [Messaging.MessageConverter.SenderId] = _nodeId
            }
        };

        await _serviceBusSender.SendMessageAsync(message, cancellationToken);
        _logger.SubscriptionAliveSent(_nodeId);
    }

    private void ProcessSubscriptionAliveMessage(ServiceBusReceivedMessage message)
    {
        var subscriptionLastActivityAt = (DateTime)message.ApplicationProperties[SubscriptionPingPropertyname];
        var subscriptionId = (string)message.ApplicationProperties[Messaging.MessageConverter.SenderId];

        _logger.SubscriptionAliveMessageReceived(_topicName, _nodeId, subscriptionId);
        _nodes[subscriptionId] = subscriptionLastActivityAt;
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs errorEventArgs)
    {
        _logger.ErrorProcess(errorEventArgs.Exception, _topicName, _nodeId, errorEventArgs.ErrorSource);
        return Task.CompletedTask;
    }

    private async Task CreateSubscriptionAsync(string topicName, string nodeId, CancellationToken cancellationToken)
    {
        if (_serviceBusProcessor is not null)
        {
            return;
        }

        await CreateTopicSubscriptionAsync(topicName, nodeId, cancellationToken);

        var processorOptions = new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false
        };

        var serviceBusProcessor = _serviceBusClient!.CreateProcessor(_topicName, _nodeId, processorOptions);

        serviceBusProcessor.ProcessMessageAsync += ReceiveMessageAsync;
        serviceBusProcessor.ProcessErrorAsync += ProcessErrorAsync;

        _serviceBusProcessor = serviceBusProcessor;
        await serviceBusProcessor.StartProcessingAsync(cancellationToken);
        _logger.SubscriptionCreated(topicName, nodeId);
    }

    private Task CreateTopicSubscriptionAsync(string topicName, string nodeId, CancellationToken cancellationToken)
    {
        const string sqlPattern = "{0} <> '{2} AND ({1} IS NULL OR {1} = '{2}')";
        const string senderIdPropertyName = Messaging.MessageConverter.SenderId;
        const string recipientIdPropertyName = Messaging.MessageConverter.SenderId;

        var subscriptionOptions = new CreateSubscriptionOptions(topicName, nodeId)
        {
            DefaultMessageTimeToLive = _serviceBusTokenTimeout
        };

        var sqlRule = string.Format(CultureInfo.InvariantCulture, sqlPattern, senderIdPropertyName, recipientIdPropertyName, nodeId);

        var filter = new SqlRuleFilter(sqlRule);
        var subscriptionRuleOptions = new CreateRuleOptions("RecipientFilter", filter);

        return _serviceBusAdministrationClient!.CreateSubscriptionAsync(subscriptionOptions, subscriptionRuleOptions, cancellationToken);
    }

    private async Task DeleteSubscriptionAsync(CancellationToken cancellationToken)
    {
        if (_nodeId is null)
        {
            return;
        }

        await DeleteSubscriptionAsync(_nodeId, cancellationToken);
        _logger.SubscriptionDeleted(_topicName, _nodeId);
        _nodeId = null;
    }

    private async Task ReceiveMessageAsync(ProcessMessageEventArgs messageEventArgs)
    {
        var message = messageEventArgs.Message;
        var cancellationToken = messageEventArgs.CancellationToken;

        if (message is null || _nodeId is null)
        {
            return;
        }

        _logger.MessageReceieved(_topicName, _nodeId, message.MessageId);

        try
        {
            if (message.ApplicationProperties.ContainsKey(SubscriptionPingPropertyname))
            {
                ProcessSubscriptionAliveMessage(message);
            }
            else
            {
                var nodeMessage = await MessageConverter.ConvertToNodeMessageAsync(message, cancellationToken);
                _observableMessages.OnNext(nodeMessage);
            }

            await messageEventArgs.CompleteMessageAsync(message, cancellationToken);
            _logger.MessageProcessed(_topicName, _nodeId, message.MessageId);
        }
        catch (Exception ex)
        {
            _logger.ErrorProccessMessage(ex, _topicName, _nodeId, message.MessageId);
            await messageEventArgs.AbandonMessageAsync(message, null, cancellationToken);
        }

    }
    #endregion
    #region Disposal Methods
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnregisterAsync().Wait();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await UnregisterAsync().ConfigureAwait(false);
        await ValueTask.CompletedTask;
    }

    ~AzureServiceBus()
    {
        Dispose(false);
    }
    #endregion
}
