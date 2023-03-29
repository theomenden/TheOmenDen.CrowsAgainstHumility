using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Azure.Cosmos.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Cosmos.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorvidCosmosServices(this IServiceCollection services, string connectionString)
    {
        services.AddPooledDbContextFactory<AppCosmosContext>(config =>
            config.UseCosmos(connectionString, "", options =>
            {
            }));

        return services;
    }
}
