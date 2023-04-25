using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Azure.Redis.HealthChecks;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Redis.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorvidRedisCaching(this IServiceCollection services, string connectionString)
    {
        services.AddHealthChecks()
            .AddCheck<RedisHealthCheck>("Redis", tags: new[] { "Azure", "Redis" });

        return services;
    }
}
