using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Discord.Notifications;

namespace TheOmenDen.CrowsAgainstHumility.Discord.Handlers;
internal sealed class MessageReceivedHandler : INotificationHandler<MessageReceivedNotification>
{
    private ILogger<MessageReceivedHandler> _logger;

    public MessageReceivedHandler(ILogger<MessageReceivedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MediatR connected - Message Received by {UserName}",
            notification.Message.Author.Username);
        return Task.CompletedTask;
    }
}
