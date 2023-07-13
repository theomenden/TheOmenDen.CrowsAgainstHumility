using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Core.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorvidProviders(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IGuidProvider, GuidProvider>();

        return services;
    }
}
