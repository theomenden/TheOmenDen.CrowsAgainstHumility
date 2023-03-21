using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.Shared.Specifications;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
public interface IRoomStateRepository: IAsyncEnumerable<RoomState>
{
    IAsyncEnumerable<RoomState> GetCurrentRoomsAsync(CancellationToken cancellationToken = default);

    IAsyncEnumerable<RoomState> GetCurrentRoomsByCriteriaAsync(Specification<RoomState> roomSpecification,
        CancellationToken cancellationToken = default);

    Task<RoomState> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default);

    Task<RoomState> GetRoomByCodeAsync(String code, CancellationToken cancellationToken = default);

    Task UpdateRoomAsync(RoomState roomToUpdate, CancellationToken cancellationToken = default);


}