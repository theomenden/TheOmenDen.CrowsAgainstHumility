using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Data.Extensions;
public static class ServiceCollectionExtensions
{
   private static readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
   {
       builder.AddJsonConsole();
       builder.AddDebug();
   });

    public static IServiceCollection AddCorvidDataServices(this IServiceCollection services, String connectionString)
    {
        services.AddPooledDbContextFactory<CrowsAgainstHumilityContext>(options =>
        {
            options.UseSqlServer(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
#if DEBUG
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
#endif
                .EnableServiceProviderCaching()
                .UseLoggerFactory(LoggerFactory);
        });

        services.AddHealthChecks()
            .AddDbContextCheck<CrowsAgainstHumilityContext>();

        return services;
    }
}

