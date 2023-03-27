using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Rest;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.Authentication;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TheOmenDen.CrowsAgainstHumility.Services.CardPoolBuilding;
using TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder;
using TheOmenDen.CrowsAgainstHumility.Services.Helpers;
using TheOmenDen.CrowsAgainstHumility.Services.Hosted;
using TheOmenDen.CrowsAgainstHumility.Services.Processing;

namespace TheOmenDen.CrowsAgainstHumility.Services.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorvidGamesServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services
            .AddScoped<IUserToPlayerProcessingService, UserToPlayerProcessingService>()
            .AddScoped<ICardPoolBuildingService, CardPoolBuildingService>()
            .AddScoped<ICardPoolFilteringService, CardPoolFilteringService>()
            .AddScoped<ICrowGameHubConnectorService, CrowGameHubConnectorService>()
            .AddScoped<IPackViewService, PackViewProcessingService>();
        return services;
    }

    public static IServiceCollection AddCorvidHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<CleanupServerJob>();
        services.AddHostedService<ReportTelemetryJob>();

        return services;
    }

    public static IServiceCollection AddCorvidDiscordServices(this IServiceCollection services)
    {
        services.AddSingleton(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers,
        });

        services.AddSingleton<DiscordSocketClient>();

        return services;
    }

    public static IServiceCollection AddCorvidTwitchServices(this IServiceCollection services,
        TwitchStrings twitchStrings)
    {
        services.AddSingleton(InitializeTwitchApi(twitchStrings));

        services.AddScoped<IPlayerVerificationService, PlayerVerificationService>();

        return services;
    }

    private static TwitchAPI InitializeTwitchApi(TwitchStrings twitchStrings)
    {
        var scopes = new List<AuthScopes>()
        {
            AuthScopes.Channel_Read,
            AuthScopes.User_Read,
            AuthScopes.Helix_User_Read_Email
        };

        return new ()
        {
            Settings =
            {
                ClientId = twitchStrings.ClientId,
                Secret = twitchStrings.Key,
                Scopes = scopes,
                SkipAutoServerTokenGeneration = false
            }
        };
    }
}
