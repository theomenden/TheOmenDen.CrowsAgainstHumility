using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Data.Repositories;

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
        services.AddEFSecondLevelCache(options =>
                options.UseMemoryCacheProvider(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(30))
                    .DisableLogging(true)
                    .CacheQueriesContainingTypes(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(30), 
                        TableTypeComparison.Contains,
                        typeof(WhiteCard), 
                        typeof(BlackCard),
                        typeof(Pack), 
                        typeof(FilteredWhiteCardsByPack), 
                        typeof(FilteredBlackCardsByPack))
                    .CacheQueriesContainingTableNames(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(30),
                        TableNameComparison.Contains,
                        "vw_FilteredWhiteCardsByPack", "vw_FilteredBlackCardsByPack", "BlackCards", "WhiteCards", "Packs")
                    .SkipCacheInvalidationCommands(commandText =>
                        commandText.Contains("NEWID()", StringComparison.InvariantCultureIgnoreCase)));

        services.AddPooledDbContextFactory<CrowsAgainstHumilityContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString, 
                    sqlOptions => sqlOptions.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
#if DEBUG
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
#endif
                .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>())
                .UseLoggerFactory(LoggerFactory);
        });

        services.AddHealthChecks()
            .AddSqlServer(connectionString)
            .AddDbContextCheck<CrowsAgainstHumilityContext>();

        services.AddScoped<IPackRepository, PackRepository>();

        return services;
    }
}

