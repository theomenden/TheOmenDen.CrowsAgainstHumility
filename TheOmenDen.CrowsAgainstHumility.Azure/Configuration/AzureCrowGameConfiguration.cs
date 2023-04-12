using TheOmenDen.CrowsAgainstHumility.Core.Models.Settings;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Configuration;
public class AzureCrowGameConfiguration : CrowGameConfiguration, IAzureCrowGameConfiguration
{
    #region Configuration Settings
    public string? ServiceBusConnectionString { get; set; }
    public string? ServiceBusTopic { get; set; }
    public int InitializationTimeout { get; set; } = 60;
    public int InitializationMessageTimeout { get; set; } = 5;
    public int SubscriptionMaintenanceInterval { get; set; } = 300;
    public int SubscriptionInactivityTimeout { get; set; } = 900;
    #endregion
    #region IAzureCrowGameConfiguration Implementations
    TimeSpan IAzureCrowGameConfiguration.InitializationTimeout => TimeSpan.FromSeconds(InitializationTimeout);
    TimeSpan IAzureCrowGameConfiguration.InitializationMessageTimeout => TimeSpan.FromSeconds(InitializationMessageTimeout);
    TimeSpan IAzureCrowGameConfiguration.SubscriptionMaintenanceInterval => TimeSpan.FromSeconds(SubscriptionMaintenanceInterval);
    TimeSpan IAzureCrowGameConfiguration.SubscriptionInactivityTimeout => TimeSpan.FromSeconds(SubscriptionInactivityTimeout);
    #endregion
}
