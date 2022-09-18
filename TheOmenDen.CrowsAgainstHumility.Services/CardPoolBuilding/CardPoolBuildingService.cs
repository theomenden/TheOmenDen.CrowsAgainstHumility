using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.CardPoolBuilding;

internal sealed class CardPoolBuildingService
{
    private readonly IDbContextFactory<CrowsAgainstHumilityContext> _dbContextFactory;

    private readonly ILogger<CardPoolBuildingService> _logger;

    public CardPoolBuildingService(IDbContextFactory<CrowsAgainstHumilityContext> dbContextFactory, ILogger<CardPoolBuildingService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<Pack>> GetPacksByNameAsync(IEnumerable<String> packNames, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var packs = await context.Packs
            .AsNoTracking()
            .Where(p => packNames.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
            .ToArrayAsync(cancellationToken);

        return packs;
    }

    public async Task<IEnumerable<Pack>> GetOfficialPacksAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var packs = await context.Packs
            .AsNoTracking()
            .Where(p => p.IsOfficialPack)
            .ToArrayAsync(cancellationToken);

        return packs;
    }
}