namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class PlayerConnections
{
    public PlayerConnections(Guid playerGuid, Guid connectionGuid)
    {
        PlayerGuid = playerGuid;
        ConnectionGuid = connectionGuid;
    }

    public Guid PlayerGuid { get; set; }

    public Guid ConnectionGuid { get; set; }
}
