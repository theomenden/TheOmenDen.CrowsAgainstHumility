using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Engine;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorvidEngineServices(this IServiceCollection services)
    {
        services.AddSingleton<ICrowGameEngine, CrowGameEngine>();

        return services;
    }
}
