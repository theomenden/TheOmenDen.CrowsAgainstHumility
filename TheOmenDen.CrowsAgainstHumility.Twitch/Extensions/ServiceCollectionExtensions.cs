using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Twitch.Channels;
using TheOmenDen.CrowsAgainstHumility.Twitch.Services;

namespace TheOmenDen.CrowsAgainstHumility.Twitch.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTwitchManagementServices(this IServiceCollection services, IConfiguration configuration)
    {
        var betterTtvUri = new Uri(configuration["BetterTTV:ConnectionString"] ?? String.Empty);

        services.AddHttpClient<IBetterTTVEmoteService, BetterTTVEmoteService>("bttvClient", options => options.BaseAddress = betterTtvUri);

        services.AddSingleton<ITwitchStrings>(new TwitchStrings(configuration["twitch-key"], configuration["twitch-clientId"]));

        services.AddScoped(typeof(IMessageChannelService<>), typeof(MessageChannelService<>))
            .AddScoped<IPlayerVerificationService, PlayerVerificationService>()
        .AddScoped<ITwitchRedemptionsManager, TwitchRedemptionsManagerService>()
        .AddScoped<ITwitchChatService, TwitchChatService>()
        .AddScoped<IBetterTTVEmoteService, BetterTTVEmoteService>();

        return services;
    }
}
