using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Configuration;
public interface IAzureCrowGameConfiguration: ICrowGameConfiguration
{
    string? ServiceBusConnectionString { get; }
    string ServiceBusTopic { get; }
    TimeSpan InitializationTimeout { get; }
    TimeSpan InitializationMessageTimeout { get; }
    TimeSpan SubscriptionMaintenanceInterval { get; }
    TimeSpan SubscriptionInactivityTimeout { get; }
}
