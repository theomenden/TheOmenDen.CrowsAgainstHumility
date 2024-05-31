using Microsoft.EntityFrameworkCore;
using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Api.Services;

internal sealed class CardService(IDbContextFactory<CrowsAgainstHumilityContext> contextFactory, ILogger<CardService> logger)
{
    public async Task<Deck<ImmutableBlackCard>> BuildBlackDeckAsync(List<ExpansionId> expansionIds, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var cards = await context.BlackCards
            .Where(x => expansionIds.Contains(x.ExpansionId))
            .Select(card => new ImmutableBlackCard(card.Id, card.Text, card.NumberOfBlanks))
            .ToListAsync(cancellationToken);

        var deck = new Deck<ImmutableBlackCard>(cards);
        deck.Shuffle();
        return deck;
    }

    public async Task<Deck<ImmutableWhiteCard>> BuildWhiteDeckAsync(List<ExpansionId> expansionIds, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var cards = await context.WhiteCards
            .Where(x => expansionIds.Contains(x.ExpansionId))
            .Select(card => new ImmutableWhiteCard(card.Id, card.Text))
            .ToListAsync(cancellationToken);

        var deck = new Deck<ImmutableWhiteCard>(cards);
        deck.Shuffle();
        return deck;
    }
}