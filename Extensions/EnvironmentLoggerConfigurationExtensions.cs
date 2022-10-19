using Serilog;
using Serilog.Configuration;
using TheOmenDen.Shared.Logging.Serilog;

namespace TheOmenDen.CrowsAgainstHumility.Extensions
{
    public static class EnvironmentLoggerConfigurationExtensions
    {
        public static LoggerConfiguration WithEventType(this LoggerEnrichmentConfiguration enrichmentConfiguration) =>
            enrichmentConfiguration is null
                ? throw new ArgumentNullException(nameof(enrichmentConfiguration))
                : enrichmentConfiguration.With<EventTypeEnricher>();
    }
}
