using System.Resources;
using System.Runtime.CompilerServices;
using Discord;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Delegates;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
using TheOmenDen.Shared.Guards;

namespace TheOmenDen.CrowsAgainstHumility.Services.Processing;

internal sealed class GameClient
{
    private readonly IRoomStateRepository _gameRepository;
    private readonly ILogger<GameClient> _logger;

    public GameClient(IRoomStateRepository gameRepository, ILogger<GameClient> logger)
    {
        _gameRepository = gameRepository;
        _logger = logger;
    }

    public async IAsyncEnumerable<RoomStateDto> GetAllRoomsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var room in _gameRepository.WithCancellation(cancellationToken))
        {
            yield return new RoomStateDto(room.Id, Enumerable.Empty<Player>())
            {
                Code = room.RoomCode,
                Name = room.Name,
            };
        }
    }

    public async Task<RoomStateDto?> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var roomToGet = await _gameRepository.GetRoomByIdAsync(roomId, cancellationToken);

        return new RoomStateDto(roomToGet.Id, Enumerable.Empty<Player>())
        {
            Code = roomToGet.RoomCode,
            Name = roomToGet.Name,
            RoomId = roomToGet.Id
        };
    }

    public async Task<RoomStateDto?> GetRoomByRoomCodeAsync(String roomCode,
        CancellationToken cancellationToken = default)
    {
        var roomToGet = await _gameRepository.GetRoomByCodeAsync(roomCode, cancellationToken);

        return new RoomStateDto(roomToGet.Id, Enumerable.Empty<Player>())
        {
            Code = roomToGet.RoomCode,
            Name = roomToGet.Name,
            RoomId = roomToGet.Id
        };
    }
}

