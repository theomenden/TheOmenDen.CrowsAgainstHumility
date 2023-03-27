using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Identity.Contexts;
using TheOmenDen.Shared.Specifications;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Repositories;

internal sealed class RoomStateRepository : IRoomStateRepository
{
    private readonly IDbContextFactory<AuthDbContext> _dbContextFactory;

    private readonly ILogger<RoomStateRepository> _logger;

    public RoomStateRepository(IDbContextFactory<AuthDbContext> dbContextFactory, ILogger<RoomStateRepository> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async IAsyncEnumerable<RoomState> GetCurrentRoomsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = from room in context.Rooms
                    join game in context.Games
                        on room.Id equals game.RoomId
                    where game.StartedAt != null
                    orderby room.Id
                    select room;

        await foreach (var room in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            yield return room;
        }
    }

    public async IAsyncEnumerable<RoomState> GetCurrentRoomsByCriteriaAsync(Specification<RoomState> roomSpecification, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = from room in context.Rooms
                    join game in context.Games
                        on room.Id equals game.RoomId
                    where roomSpecification.IsSatisfiedBy(room)
                    select room;

        await foreach (var room in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            yield return new RoomState
            {

            };
        }
    }

    public async Task<RoomState> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId, cancellationToken);

        return room;
    }

    public async Task<RoomState> GetRoomByCodeAsync(String code, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var room = await context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == code, cancellationToken);

        return room;
    }

    public async Task UpdateRoomAsync(RoomState roomToUpdate, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        try
        {
            context.Rooms.Update(roomToUpdate);

            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Could not persist current room state {@Ex}", ex);
        }
    }

    public async IAsyncEnumerator<RoomState> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await foreach (var room in context.Rooms)
        {
            yield return room;
        }
    }
}
