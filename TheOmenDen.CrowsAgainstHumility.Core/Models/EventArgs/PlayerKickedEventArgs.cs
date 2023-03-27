using System.Security.Cryptography.X509Certificates;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
public sealed class PlayerKickedEventArgs: CrowGameEventArgs
{
    public PlayerKickedEventArgs(Guid serverId, Player player) :base(serverId) => KickedPlayer = player;

    public Player KickedPlayer { get; }
}
