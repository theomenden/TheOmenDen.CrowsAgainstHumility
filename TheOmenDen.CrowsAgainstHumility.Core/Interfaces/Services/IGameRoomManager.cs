using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IGameRoomManager
{
    Task<RoomStateDto> AddPlayerToRoomAsync(Guid roomId, Player player, string connectionId);
    Task<RoomStateDto?> DisconnectAsync(string connectionId);
    Task<RoomStateDto?> UpdateRoomAsync(RoomOptions roomOptions, string connectionId);
    Task<RoomStateDto?> UpdatePlayerAsync(PlayerOptions playerOptions, string connectionId);
    Task<RoomStateDto?> PlayWhiteCardAsync(WhiteCard card, string connectionId);
    Task<RoomStateDto?> NextBlackCardAsync(Guid roomId, BlackCard card);
    Task<RoomStateDto?> ResetGameBoardAsync(string connectionId);
    Task<GameRoles> GetNewCardTsarAsync(Guid roomId);
}
