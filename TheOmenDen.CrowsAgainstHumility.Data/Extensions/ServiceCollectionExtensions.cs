using EasyCaching.InMemory;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Data.Extensions;

public static class ServiceCollectionExtensions
{
    private const string providerName = "CAH_InMemory";
    public static IServiceCollection AddCorvidCardData(this IServiceCollection services, string connectionString)
    {

        services.AddEFSecondLevelCache(options =>
            options.UseEasyCachingCoreProvider(providerName, isHybridCache: false)
                .ConfigureLogging(true)
                .UseCacheKeyPrefix("CAH_")
                .UseDbCallsIfCachingProviderIsDown(TimeSpan.FromMinutes(1))
                .CacheAllQueriesExceptContainingTableNames(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(30), "Expansions", "Players"));

        services.AddEasyCaching(options =>
        {
            options.UseInMemory(config =>
            {
                config.DBConfig = new InMemoryCachingOptions
                {
                    // scan time, default is 60s
                    ExpirationScanFrequency = 60, // seconds
                    // total count of cache items, default is 10000
                    SizeLimit = 1000,
                    EnableReadDeepClone = false,
                    EnableWriteDeepClone = false
                };
            });
        });

        services.AddPooledDbContextFactory<CrowsAgainstHumilityContext>((serviceProvider, options) =>
        {
            options
                .ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverter>()
                .UseSqlServer(connectionString, options =>
                {
                    options
                        .CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds)
                        .EnableRetryOnFailure()
                        .MigrationsAssembly(typeof(CrowsAgainstHumilityContext).Assembly.FullName);
                })
                .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>())
#if DEBUG
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
#endif
                .EnableServiceProviderCaching();
        });
    }
}