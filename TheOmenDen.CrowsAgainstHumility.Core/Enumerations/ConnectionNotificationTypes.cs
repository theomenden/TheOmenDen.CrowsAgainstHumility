using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record ConnectionNotificationTypes : EnumerationBase<ConnectionNotificationTypes>
{
    private ConnectionNotificationTypes(string name, int id) : base(name, id) { }

    public static readonly ConnectionNotificationTypes Closed = new(nameof(Closed), 200);
    public static readonly ConnectionNotificationTypes Reconnecting = new(nameof(Reconnecting), 300);
    public static readonly ConnectionNotificationTypes Connected = new(nameof(Connected), 400);
}
