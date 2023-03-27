using Discord.WebSocket;
using MediatR;

namespace TheOmenDen.CrowsAgainstHumility.Discord.Notifications;
internal sealed record MessageReceivedNotification: INotification
{
    public MessageReceivedNotification(SocketMessage message)
    {
      Message = message ?? throw new ArgumentNullException(nameof(message));   
    }

    public SocketMessage Message { get; }
}
