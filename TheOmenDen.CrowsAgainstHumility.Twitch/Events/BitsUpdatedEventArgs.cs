namespace TheOmenDen.CrowsAgainstHumility.Twitch.Events;

internal sealed class BitsUpdatedEventArgs: EventArgs
{
    public Int32 BitsRedeemed { get; set; }
}
