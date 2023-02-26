using Microsoft.Extensions.DependencyInjection;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Twitch.Channels;
using TheOmenDen.CrowsAgainstHumility.Twitch.Services;

namespace TheOmenDen.CrowsAgainstHumility.Twitch.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTwitchManagementServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IMessageChannelService<>),typeof(MessageChannelService<>))
        .AddScoped<ITwitchRedemptionsManager, TwitchRedemptionsManagerService>()
        .AddScoped<ITwitchChatService, TwitchChatService>();
        
        return services;
    }
}
