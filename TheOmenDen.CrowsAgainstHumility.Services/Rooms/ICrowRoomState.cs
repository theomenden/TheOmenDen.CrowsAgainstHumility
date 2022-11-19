using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.Rooms;
internal interface ICrowRoomState
{
    Task Enter(CancellationToken cancellationToken = default);
    Task AddCrow(Player player, bool isReconnection, CancellationToken cancellationToken = default);
    Task RemoveCrow(Player player, CancellationToken cancellationToken = default);
}
