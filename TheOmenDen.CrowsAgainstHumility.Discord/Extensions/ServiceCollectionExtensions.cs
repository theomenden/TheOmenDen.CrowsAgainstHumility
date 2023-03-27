using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Discord.Services;

namespace TheOmenDen.CrowsAgainstHumility.Discord.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscordServices(this IServiceCollection services, DiscordStrings discordStrings)
    {
        services.AddMediatR(options => options.RegisterServicesFromAssemblies(typeof(DiscordListener).Assembly));

        services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
        {
            AlwaysDownloadUsers = true,
            MessageCacheSize = 100,
            GatewayIntents = GatewayIntents.AllUnprivileged,
            LogLevel = LogSeverity.Info,
            UseSystemClock = true
        }))
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));

        return services;
    }
}
