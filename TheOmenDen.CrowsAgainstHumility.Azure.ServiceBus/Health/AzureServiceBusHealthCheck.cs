using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Nodes;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Health;
public sealed class AzureServiceBusHealthCheck : IHealthCheck
{
    private readonly CrowGameAzureNode _node;
    private readonly Lazy<ServiceBusAdministrationClient> _serviceBusAdministrationClient;

    public AzureServiceBusHealthCheck(CrowGameAzureNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        _node = node;
        _serviceBusAdministrationClient = new (() => new(_node.Configuration.ServiceBusConnectionString));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var configuration = _node.Configuration;
            var topicName = configuration.ServiceBusTopic;

            if (String.IsNullOrEmpty(topicName))
            {
                topicName = "Crows Against Humility";
            }

            var properties = await _serviceBusAdministrationClient.Value.GetSubscriptionRuntimePropertiesAsync(topicName, _node.NodeId, cancellationToken);

            return HealthCheckResult.Healthy($"Azure ServiceBus Subscription has {properties.Value.ActiveMessageCount} active messages");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Azure ServiceBus Subscription is unhealthy", ex);
        }
    }
}
