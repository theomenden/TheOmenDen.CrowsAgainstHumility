using Serilog;
using Serilog.Configuration;
using TheOmenDen.Shared.Logging.Serilog;

namespace TheOmenDen.CrowsAgainstHumility.Extensions
{
    public static class EnvironmentLoggerConfigurationExtensions
    {
        public static LoggerConfiguration WithEventType(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if(enrichmentConfiguration is null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }

            return enrichmentConfiguration.With<EventTypeEnricher>();
        }
    }
}
