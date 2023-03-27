
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using TheOmenDen.CrowsAgainstHumility.Discord.Notifications;

namespace TheOmenDen.CrowsAgainstHumility.Discord.Services;
internal sealed class DiscordListener
{
    private readonly CancellationToken _cancellationToken;

    private readonly DiscordSocketClient _discordSocketClient;

    private readonly ILogger<DiscordListener> _logger;

    private readonly IMediator _mediator;

    public DiscordListener(DiscordSocketClient discordSocketClient, ILogger<DiscordListener> logger, IMediator mediator, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _discordSocketClient = discordSocketClient;
        _logger = logger;
        _mediator = mediator;
    }

    public Task StartAsync()
    {
        _discordSocketClient.Ready += OnReadyAsync;
        _discordSocketClient.MessageReceived += OnMessageReceivedAsync;

        return Task.CompletedTask;
    }

    private Task OnMessageReceivedAsync(SocketMessage socketMessage)
    {
        _logger.LogInformation("Message recieved {Message}", socketMessage.Id);
        return _mediator.Publish(new MessageReceivedNotification(socketMessage), _cancellationToken);
    }

    private Task OnReadyAsync()
    {
        return _mediator.Publish(ReadyNotification.Default, _cancellationToken);
    }
}
