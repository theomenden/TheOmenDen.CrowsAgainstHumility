using System.Collections;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Services.Helpers;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.Shared.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Services.CardPoolBuilding;

internal sealed class CardPoolFilteringService: IDisposable, IAsyncDisposable, ICardPoolFilteringService
{
    private const string OfficialFilteredCards = nameof(OfficialFilteredCards);
    private readonly ILogger<CardPoolFilteringService> _logger;

    private readonly IDbContextFactory<CrowsAgainstHumilityContext> _dbContextFactory;

    private readonly IDistributedCache _distributedCache;

    public CardPoolFilteringService(ILogger<CardPoolFilteringService> logger, IDbContextFactory<CrowsAgainstHumilityContext> dbContextFactory, IDistributedCache distributedCache)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _distributedCache = distributedCache;
    }

    public async Task<IEnumerable<WhiteCard>> GetDefaultFiltersOnOfficialWhiteCardsAsync(CancellationToken cancellationToken = default)
    {
        var cachedCards = await _distributedCache.GetRecordAsync<IEnumerable<WhiteCard>>(OfficialFilteredCards);

        if (cachedCards is not null && cachedCards.TryGetNonEnumeratedCount(out var countCachedCards))
        {
            _logger.LogInformation("Found {Count} cached filtered cards", countCachedCards);
            return cachedCards.ToImmutableArray();
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var filteredCardsQuery = from filteredCard in context.vw_FilteredWhiteCardsByPack
                                 join pack in context.Packs
                                     on filteredCard.PackId equals pack.Id
                                 where pack.IsOfficialPack
                                 select new WhiteCard
                                 {
                                     Id = filteredCard.Id,
                                     PackId = filteredCard.PackId,
                                     CardText = filteredCard.CardText,
                                 };

        var filteredCards = await filteredCardsQuery.ToArrayAsync(cancellationToken);

        await _distributedCache.SetRecordAsync<WhiteCard[]>(OfficialFilteredCards, filteredCards);

        return filteredCards;
    }

    public async IAsyncEnumerable<WhiteCard> GetDefaultFiltersOnOfficialWhiteCardsStreamAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var filteredCardsQuery = from filteredCard in context.vw_FilteredWhiteCardsByPack
                                 join pack in context.Packs
                                     on filteredCard.PackId equals pack.Id
                                 where pack.IsOfficialPack
                                 select new WhiteCard
                                 {
                                     Id = filteredCard.Id,
                                     PackId = filteredCard.PackId,
                                     CardText = filteredCard.CardText,
                                 };

        await foreach (var card in filteredCardsQuery.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            yield return card;
        }
    }

    public async Task<IEnumerable<WhiteCard>> GetDefaultFiltersOnWhiteCardsAsync(IEnumerable<Guid> cardPackIds, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var filteredCards = context.vw_FilteredWhiteCardsByPack
            .Where(fc => cardPackIds.Contains(fc.PackId))
            .Select(fc => new WhiteCard
            {
                Id = fc.Id,
                PackId = fc.PackId,
                CardText = fc.CardText
            });

        return await filteredCards.ToArrayAsync(cancellationToken);
    }

    public async IAsyncEnumerable<WhiteCard> GetDefaultFiltersOnWhiteCardsStreamAsync(IEnumerable<Guid> cardPackIds, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await foreach (var card in context.vw_FilteredWhiteCardsByPack
                           .Where(fc => cardPackIds.Contains(fc.PackId))
                           .Select(fc => new WhiteCard
                           {
                               Id = fc.Id,
                               PackId = fc.PackId,
                               CardText = fc.CardText
                           })
                           .AsAsyncEnumerable()
                           .WithCancellation(cancellationToken))
        {
            yield return card;
        }
    }

    public async Task<IEnumerable<WhiteCard>> GetFilteredWhiteCardsAsync(String filterValue,
        IEnumerable<Guid> cardPackIds, CancellationToken cancellationToken = default)
    {
        return await GetFilteredWhiteCardsAsync(new [] { filterValue }, cardPackIds, cancellationToken);
    }

    public async Task<IEnumerable<WhiteCard>> GetFilteredWhiteCardsAsync(IEnumerable<String> filterValues,
        IEnumerable<Guid> cardPackIds, CancellationToken cancellationToken = default)
    {
        var defaultFilteredCards = await GetDefaultFiltersOnWhiteCardsAsync(cardPackIds, cancellationToken);
        
        var filteredCards = from card in defaultFilteredCards
            from filterValue in filterValues
            where !card.CardText.Contains(filterValue, StringComparison.OrdinalIgnoreCase)
            select card;

        return filteredCards.ToArray();
    }

    public void Dispose()
    {
        if (StringBuilderPoolFactory<CardPoolFilteringService>.Exists(nameof(CardPoolFilteringService)))
        {
            StringBuilderPoolFactory<CardPoolFilteringService>.Remove(nameof(CardPoolFilteringService));
        }

        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        if (StringBuilderPoolFactory<CardPoolFilteringService>.Exists(nameof(CardPoolFilteringService)))
        {
            StringBuilderPoolFactory<CardPoolFilteringService>.Remove(nameof(CardPoolFilteringService));
        }

        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}

