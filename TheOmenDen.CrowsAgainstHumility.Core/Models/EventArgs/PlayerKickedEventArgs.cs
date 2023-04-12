namespace TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
public sealed class PlayerKickedEventArgs: CrowGameEventArgs
{
    public PlayerKickedEventArgs(Guid serverId, Observer player) :base(serverId) => KickedPlayer = player;

    public Observer KickedPlayer { get; }
}
