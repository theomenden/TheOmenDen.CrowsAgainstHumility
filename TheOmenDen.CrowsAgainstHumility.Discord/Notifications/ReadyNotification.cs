using MediatR;

namespace TheOmenDen.CrowsAgainstHumility.Discord.Notifications;
internal sealed record ReadyNotification: INotification
{
    private ReadyNotification()
    {

    }

    public static readonly ReadyNotification Default = new();
}
