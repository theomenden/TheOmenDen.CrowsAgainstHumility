using TheOmenDen.CrowsAgainstHumility.Azure.Messages;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Interfaces;
public interface IServiceBus
{
    IObservable<NodeMessage> ObservableMessages { get; }
    Task SendMessageAsync(NodeMessage message, CancellationToken cancellationToken = default);
    Task RegisterAsync(string nodeId, CancellationToken cancellationToken = default);
    Task UnregisterAsync(CancellationToken cancellationToken = default);
}
