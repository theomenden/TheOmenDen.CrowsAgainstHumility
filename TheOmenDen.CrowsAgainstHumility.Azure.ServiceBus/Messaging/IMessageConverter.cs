using Azure.Messaging.ServiceBus;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Nodes;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging;
public interface IMessageConverter
{
    ValueTask<ServiceBusMessage> ConvertToServiceBusMessageAsync(ObjectNodeMessage message, CancellationToken cancellationToken = default);
    ValueTask<ObjectNodeMessage> ConvertToNodeMessageAsync(ServiceBusReceivedMessage message, CancellationToken cancellationToken = default);
}
