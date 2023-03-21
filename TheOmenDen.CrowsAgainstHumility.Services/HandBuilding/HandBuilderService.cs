using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Data.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.HandBuilding;

internal sealed class HandBuilderService
{
    private static CrowGame _game;

    private readonly IDbContextFactory<CrowsAgainstHumilityContext> _dbContextFactory;

    private readonly IDistributedCache _distributedCache;

    private readonly ILogger<HandBuilderService> _logger;

    public HandBuilderService(IDbContextFactory<CrowsAgainstHumilityContext> dbContextFactory, ILogger<HandBuilderService> logger, IDistributedCache distributedCache)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _distributedCache = distributedCache;
    }

    public async Task GetInitialCardsForGame(CrowGame game, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
    }
}
