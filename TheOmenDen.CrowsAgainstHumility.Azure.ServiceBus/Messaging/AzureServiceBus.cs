using System.Collections.Concurrent;
using System.Reactive.Subjects;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Buses;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Nodes;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging;
internal sealed class AzureServiceBus: IServiceBus, IDisposable
{
    private const string DefaultTopicName = "CrowGame";
    private const string SubscriptionPingPropertyName = "SubscriptionPing";

    private static readonly TimeSpan _serviceBusTokenTimeout = TimeSpan.FromMinutes(1);

    private readonly Subject<NodeMessage<T>> _observableMessages = new Subject<NodeMessage<T>>();
    private readonly ConcurrentDictionary<string, DateTime> _nodes = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<AzureServiceBus> _logger;

    private volatile string? _nodeId;
    private string? _connectionString;
    private string? _topicName;
    private ServiceBusAdministrationClient _serviceBusAdministrationClient;
    private ServiceBusClient? _serviceBusClient;
     private ServiceBusSender? _serviceBusSender;
     private ServiceBusProcessor? _serviceBusProcessor;


    public IObservable<NodeMessage<T>> GetObservableMessages<T>()
    {
        throw new NotImplementedException();
    }

    public async Task SendMessageAsync<T>(NodeMessage<T> message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task RegisterAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task UnregisterAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
