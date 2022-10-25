using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Services.Exceptions;
using TheOmenDen.CrowsAgainstHumility.Services.Helpers;

namespace TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder;

internal sealed class CrowGameService : ICrowGameService
{
    private readonly ICrowGameRepository _crowGameRepository;

    private readonly IDbContextFactory<CrowsAgainstHumilityContext> _dbContextFactory;

    private readonly ILogger<CrowGameService> _logger;

    private readonly IDistributedCache _distributedCache;

    public CrowGameService(ICrowGameRepository crowGameRepository, IDbContextFactory<CrowsAgainstHumilityContext> dbContextFactory, ILogger<CrowGameService> logger, IDistributedCache distributedCache)
    {
        _crowGameRepository = crowGameRepository;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _distributedCache = distributedCache;
    }

    public async Task<IEnumerable<CrowGame>> GetCrowGamesAsync(CancellationToken cancellationToken = default)
    {
        var crowGames = await _crowGameRepository.GetCrowGamesAsync(cancellationToken);

        return crowGames ?? Enumerable.Empty<CrowGame>();
    }

    public async Task<CrowGameDto> GetCrowGameById(Guid id, CancellationToken cancellationToken = default)
    {
        var crowGame = await _crowGameRepository.WithIdAsync(id, cancellationToken);

        if (crowGame is null)
        {
            throw new CrowGameNotFoundException($"Couldn't find {nameof(crowGame)}: {id}");
        }

        var crowGameDto = await _distributedCache.GetRecordAsync<CrowGameDto>(crowGame.LobbyCode);

        if (crowGameDto is not null)
        {
            _logger.LogInformation("Found Game with Lobby Code: {LobbyCode}", crowGame.LobbyCode);
            return crowGameDto;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var crowGamePackIds = crowGame.GameCardPacks.Select(x => x.PackId);

        var packsUsed = await context.Packs.Where(p => crowGamePackIds.Contains(p.Id))
            .Include(p => p.WhiteCards)
            .Include(p => p.BlackCards)
            .ToArrayAsync(cancellationToken);

        var playersInGame = crowGame.Players.Select(player => new Player
        {
            Username = player.Player.UserName,
            IsCardCzar = false,
            AwesomePoints = 2
        });

        crowGameDto = new CrowGameDto
        (
            packsUsed,
            playersInGame,
            crowGame.LobbyCode,
            crowGame.Name,
            crowGame.Id
        );

        await _distributedCache.SetRecordAsync(crowGameDto.LobbyCode, crowGameDto);

        return crowGameDto;
    }
    
    public async Task CreateCrowGameAsync(CrowGameCreator crowGameCreator, CancellationToken cancellationToken = default)
    {
        await _crowGameRepository.CreateRoomAsync(crowGameCreator.RoomCode,
            crowGameCreator.GameName,
            crowGameCreator.CreatorUserId,
            crowGameCreator.Packs,
            cancellationToken);
    }
}
