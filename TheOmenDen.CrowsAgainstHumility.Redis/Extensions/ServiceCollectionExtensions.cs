using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Azure.Interfaces;
using TheOmenDen.CrowsAgainstHumility.Redis.HealthChecks;
using TheOmenDen.CrowsAgainstHumility.Redis.ServiceBus;

namespace TheOmenDen.CrowsAgainstHumility.Redis.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorvidRedisCaching(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IServiceBus, RedisServiceBus>();

        services.AddHealthChecks()
            .AddCheck<RedisHealthCheck>("Redis", tags: new []{"Azure", "Redis"});

        return services;
    }
}
