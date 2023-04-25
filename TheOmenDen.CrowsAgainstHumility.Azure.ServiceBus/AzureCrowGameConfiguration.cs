using TheOmenDen.CrowsAgainstHumility.Core.Models.Settings;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus;
public class AzureCrowGameConfiguration: CrowGameConfiguration, IAzureCrowGameConfiguration
{
    public string? ServiceBusConnectionString { get; set; } = String.Empty;
    public string? ServiceBusTopic { get; } = String.Empty;
    public int InitializationTimeout { get; set; } = 60;
    public int InitializationMessageTimeout { get; set; } = 5;
    public int SubscriptionMaintenanceInterval { get; set; } = 300;
    public int SubscriptionInactivityTimeout { get; set; } = 900;

    TimeSpan IAzureCrowGameConfiguration.InitializationTimeout => TimeSpan.FromSeconds(InitializationTimeout);
    TimeSpan IAzureCrowGameConfiguration.InitializationMessageTimeout => TimeSpan.FromSeconds(InitializationMessageTimeout);
    TimeSpan IAzureCrowGameConfiguration.SubscriptionMaintenanceInterval => TimeSpan.FromSeconds(SubscriptionMaintenanceInterval);
    TimeSpan IAzureCrowGameConfiguration.SubscriptionInactivityTimeout => TimeSpan.FromSeconds(SubscriptionInactivityTimeout);
}
