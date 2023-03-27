using TheOmenDen.CrowsAgainstHumility.Core.Models.Azure;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Redis;
public interface IServiceBus
{
    IObservable<NodeMessage> ObservableMessages { get; }
    Task SendMessageAsync(NodeMessage message, CancellationToken cancellationToken = default);
    Task RegisterAsync(string nodeId, CancellationToken cancellationToken = default);
    Task UnRegisterAsync(CancellationToken cancellationToken = default);
}
