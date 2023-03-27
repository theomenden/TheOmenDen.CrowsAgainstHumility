namespace TheOmenDen.CrowsAgainstHumility.Events;
public sealed class UserDisconnectEventArgs: EventArgs
{
    public String UserId { get; init; } = String.Empty;
}
