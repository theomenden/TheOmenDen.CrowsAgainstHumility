using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface ICrowGameHubConnectorService
{
    event EventHandler<RoomStateDto>? RoomStateUpdated;
    bool IsConnected { get; }
    Task OpenAsync();
    Task JoinRoomAsync(Guid roomId, Player player);
    Task PlayWhiteCardAsync(WhiteCard card);
    Task UpdateRoomAsync(RoomOptions options);
    Task UpdatePlayerAsync(PlayerOptions options);
    Task ResetGameBoardAsync();
}
