using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Nodes;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Buses;
public interface IServiceBus
{
    IObservable<NodeMessage<T>> GetObservableMessages<T>();
    Task SendMessageAsync<T>(NodeMessage<T> message, CancellationToken cancellationToken = default);
    Task RegisterAsync(string nodeId, CancellationToken cancellationToken = default);
    Task UnregisterAsync(CancellationToken cancellationToken = default);
}
