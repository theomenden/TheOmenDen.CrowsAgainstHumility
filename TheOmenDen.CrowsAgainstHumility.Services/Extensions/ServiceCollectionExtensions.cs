using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Services.Hosted;

namespace TheOmenDen.CrowsAgainstHumility.Services.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorvidGeneralServices(this IServiceCollection services)
    {
        services.AddHostedService<CleanupServerJob>();
        services.AddHostedService<ReportTelemetryJob>();

        return services;
    }
}
