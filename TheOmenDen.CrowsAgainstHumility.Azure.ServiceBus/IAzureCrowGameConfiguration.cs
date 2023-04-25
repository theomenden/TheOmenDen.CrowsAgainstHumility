using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus;
public interface IAzureCrowGameConfiguration: ICrowGameConfiguration
{
    string? ServiceBusConnectionString { get; }
    string? ServiceBusTopic { get; }

    TimeSpan InitializationTimeout { get; }
    TimeSpan InitializationMessageTimeout { get; }
    TimeSpan SubscriptionMaintenanceInterval { get; }
    TimeSpan SubscriptionInactivityTimeout { get; }
}
