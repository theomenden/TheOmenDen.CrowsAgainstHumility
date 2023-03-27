using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
public sealed class RoomUpdatedEventArgs: CrowGameEventArgs
{
    public RoomUpdatedEventArgs(Guid serverId, CrowGameServer updatedCrowGameServer)
    :base(serverId) => UpdatedServer = updatedCrowGameServer;

    public CrowGameServer UpdatedServer { get; }
}
