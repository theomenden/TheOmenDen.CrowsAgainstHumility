using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using Microsoft.Extensions.Caching.Distributed;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Services.Helpers;
using System.Runtime.CompilerServices;
using TheOmenDen.Shared.Utilities;

namespace TheOmenDen.CrowsAgainstHumility.Services.CardPoolBuilding;

internal sealed class CardPoolBuildingService : ICardPoolBuildingService
{
    private readonly IDbContextFactory<CrowsAgainstHumilityContext> _dbContextFactory;

    private readonly IDistributedCache _distributedCache;

    private readonly ILogger<CardPoolBuildingService> _logger;

    public CardPoolBuildingService(IDbContextFactory<CrowsAgainstHumilityContext> dbContextFactory, ILogger<CardPoolBuildingService> logger, IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async IAsyncEnumerable<Pack> GetPacksBySearchValueAsync(String searchValue, [EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var packsQuery = context.Packs.AsQueryable();

        if (!String.IsNullOrWhiteSpace(searchValue))
        {
            packsQuery = packsQuery
                .Where(p => p.Name.StartsWith(searchValue));
        }

        var packsAsyncStream = packsQuery
            .TagWith("Search Results Query")
            .Select(pack => new Pack
            {
                Id = pack.Id,
                Name = pack.Name,
                IsOfficialPack = pack.IsOfficialPack
            })
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken);

        await foreach(var pack in packsAsyncStream)
        {
            yield return pack;
        }
    }

    public async IAsyncEnumerable<Pack> GetRandomPacksAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var skippedRows = ThreadSafeRandom.Global.Next(0, 130);
        var rowsToTake = ThreadSafeRandom.Global.Next(5, 10);

        await foreach (var pack in context.Packs
                           .AsNoTracking()
                           .TagWith("Random Packs Query")
                           .Skip(skippedRows)
                           .Take(rowsToTake)
                           .Select(pack => new Pack
                           {
                               Id = pack.Id,
                               Name = pack.Name,
                               IsOfficialPack = pack.IsOfficialPack
                           })
                           .AsAsyncEnumerable()
                           .WithCancellation(cancellationToken))
        {
            yield return pack;
        }

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

        var cachedData = await _distributedCache.GetRecordAsync<IEnumerable<Pack>>("officialPacks");

        var packs = cachedData?.ToArray() ?? Array.Empty<Pack>();

        if (packs.Any())
        {
            _ = packs.TryGetNonEnumeratedCount(out var count);

            _logger.LogInformation("Found {Count} cached packs", count);

            return packs;
        }
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        packs = await context.Packs
            .AsNoTracking()
            .Where(p => p.IsOfficialPack)
            .Select(p => new Pack
            {
                Id = p.Id,
                Name = p.Name,
                IsOfficialPack = p.IsOfficialPack
            })
            .ToArrayAsync(cancellationToken);

        await _distributedCache.SetRecordAsync<IEnumerable<Pack>>("officialPacks", packs);

        return packs;
    }

    public async Task<IEnumerable<Pack>> GetOfficialPacksWithCardsAsync(CancellationToken cancellationToken = default)
    {

        var cachedData = await _distributedCache.GetRecordAsync<IEnumerable<Pack>>("officialPacks");

        var packs = Array.Empty<Pack>();

        if (cachedData?.Any() is not true)
        {
            return packs;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        packs = await context.Packs
            .AsNoTracking()
            .Include(p => p.BlackCards)
            .Include(p => p.WhiteCards)
            .Where(p => p.IsOfficialPack)

            .ToArrayAsync(cancellationToken);

        await _distributedCache.SetRecordAsync<IEnumerable<Pack>>("officialPacks", packs);

        return packs;
    }
}