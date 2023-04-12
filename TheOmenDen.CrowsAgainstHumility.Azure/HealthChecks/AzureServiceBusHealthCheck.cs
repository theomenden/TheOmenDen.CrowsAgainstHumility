using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Azure;

namespace TheOmenDen.CrowsAgainstHumility.Azure.HealthChecks;
internal sealed class AzureServiceBusHealthCheck: IHealthCheck
{
    private readonly CrowGameAzureNode _node;
    private readonly Lazy<ServiceBusAdministrationClient> _serviceBusAdministrationClient;

    public AzureServiceBusHealthCheck(CrowGameAzureNode node)
    {
        _node = node ?? throw new ArgumentNullException(nameof(node));

        _serviceBusAdministrationClient = new Lazy<ServiceBusAdministrationClient>(() =>
            new ServiceBusAdministrationClient(node.Configuration.ServiceBusConnectionString));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var configuration = _node.Configuration;
            var topicName = configuration.ServiceBusTopic;

            if (String.IsNullOrWhiteSpace(topicName))
            {
                topicName = "CrowsAgainstHumility";
            }

            var properties = await _serviceBusAdministrationClient.Value.GetSubscriptionRuntimePropertiesAsync(topicName, _node.NodeId, cancellationToken);
            return HealthCheckResult.Healthy($"Azure ServiceBus Subscription has {properties.Value.ActiveMessageCount} active messages.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Azure ServiceBus Subscription is currently unhealthy", ex);
        }
    }
}
