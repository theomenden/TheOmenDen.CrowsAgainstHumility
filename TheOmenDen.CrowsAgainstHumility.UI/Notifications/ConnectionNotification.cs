using Blazorise;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Notifications;

public class ConnectionNotification
{
    public ConnectionNotification(ConnectionNotificationTypes type)
    {
        Type = type;

        type.When(ConnectionNotificationTypes.Closed)
                .Then(() =>
                {
                    Text = ConnectionNotificationTypes.Closed.Name;
                    Icon = IconName.TimesCircle;
                    AlertBackground = Background.Danger;
                })
            .When(ConnectionNotificationTypes.Reconnecting)
                .Then(() =>
                {
                    Text = ConnectionNotificationTypes.Reconnecting.Name;
                    Icon = IconName.Clock;
                    AlertBackground = Background.Warning;
                })
            .When(ConnectionNotificationTypes.Connected)
                .Then(() =>
                {
                    Text = ConnectionNotificationTypes.Closed.Name;
                    Icon = IconName.CheckCircle;
                    AlertBackground = Background.Success;
                })
            .DefaultCondition(() => throw new ArgumentOutOfRangeException(nameof(type), type, null));
    }

    public IconName Icon { get; private set; }
    public String Text { get; private set; }
    public Background AlertBackground { get; private set; }
    public ConnectionNotificationTypes Type { get; }
}
