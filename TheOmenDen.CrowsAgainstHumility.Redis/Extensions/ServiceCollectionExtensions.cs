using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Azure.Interfaces;
using TheOmenDen.CrowsAgainstHumility.Azure.Redis.HealthChecks;
using TheOmenDen.CrowsAgainstHumility.Azure.Redis.ServiceBus;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Redis.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorvidRedisCaching(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IServiceBus, RedisServiceBus>();

        services.AddHealthChecks()
            .AddCheck<RedisHealthCheck>("Redis", tags: new[] { "Azure", "Redis" });

        return services;
    }
}
