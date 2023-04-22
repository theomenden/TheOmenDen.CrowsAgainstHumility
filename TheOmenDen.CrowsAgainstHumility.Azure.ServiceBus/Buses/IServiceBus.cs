using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Nodes;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Buses;
public interface IServiceBus
{
    IObservable<ObjectNodeMessage> GetObservableMessages();
    Task SendMessageAsync(ObjectNodeMessage  message, CancellationToken cancellationToken = default);
    Task RegisterAsync(string nodeId, CancellationToken cancellationToken = default);
    Task UnregisterAsync(CancellationToken cancellationToken = default);
}
