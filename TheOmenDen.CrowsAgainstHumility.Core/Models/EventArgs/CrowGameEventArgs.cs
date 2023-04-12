namespace TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
public abstract class CrowGameEventArgs
{
    protected CrowGameEventArgs(Guid serverId)
    {
        ServerId = serverId;
    }
    public Guid ServerId { get; }
}
