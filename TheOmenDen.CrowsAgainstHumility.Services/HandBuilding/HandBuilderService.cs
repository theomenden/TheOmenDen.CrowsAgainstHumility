using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Data.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.HandBuilding;

internal sealed class HandBuilderService
{
    private static CrowGame _game;

    private readonly IDbContextFactory<CrowsAgainstHumilityContext> _dbContextFactory;

    private readonly ILogger<HandBuilderService> _logger;

    public HandBuilderService(IDbContextFactory<CrowsAgainstHumilityContext> dbContextFactory, ILogger<HandBuilderService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task GetInitialCardsForGame(CrowGame game, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var initialWhiteCardPool = context.WhiteCards
            .Where(wc => game.UsedPacks.Contains(wc.Pack))
            .Take(game.Players.Count() * 100);
    }
}
