using Azure.Messaging.ServiceBus;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging.Messages;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Nodes;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging;
public interface IMessageConverter
{
    ValueTask<ServiceBusMessage> ConvertToServiceBusMessageAsync(NodeMessage message, CancellationToken cancellationToken = default);
    ValueTask<NodeMessage<ILobbyMessage>> ConvertToNodeMessageAsync(ServiceBusReceivedMessage message, CancellationToken cancellationToken = default);
}
