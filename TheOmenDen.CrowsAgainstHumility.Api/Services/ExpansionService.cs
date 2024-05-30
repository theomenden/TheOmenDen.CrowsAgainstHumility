using Microsoft.EntityFrameworkCore;
using TheOmenDen.CrowsAgainstHumility.Core.Identifiers;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Api.Services;

internal sealed class ExpansionService(IDbContextFactory<CrowsAgainstHumilityContext> contextFactory)
{
    public async Task<Deck<BlackCard>> BuildBlackDeckAsync(List<ExpansionId> expansionIds, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var cards = await context.BlackCards
            .Where(x => expansionIds.Contains(x.ExpansionId))
            .ToListAsync(cancellationToken);

        var deck = new Deck<BlackCard>();
        foreach (var card in cards)
        {
            deck.AddCard(card);
        }
        deck.Shuffle();
        return deck;
    }

    public async Task<Deck<WhiteCard>> BuildWhiteDeckAsync(List<ExpansionId> expansionIds, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var cards = await context.WhiteCards
            .Where(x => expansionIds.Contains(x.ExpansionId))
            .ToListAsync(cancellationToken);

        var deck = new Deck<WhiteCard>();
        foreach (var card in cards)
        {
            deck.AddCard(card);
        }
        deck.Shuffle();
        return deck;
    }


    public async Task<IEnumerable<Expansion>> GetExpansionsAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Expansions.ToListAsync(cancellationToken);
    }

    public async Task<Expansion> GetExpansionByIdAsync(ExpansionId id, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Expansions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Expansion> CreateExpansionAsync(Expansion expansion, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        context.Expansions.Add(expansion);
        await context.SaveChangesAsync(cancellationToken);
        return expansion;
    }

    public async Task UpdateExpansionAsync(Expansion expansion, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        context.Expansions.Update(expansion);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteExpansionAsync(ExpansionId id, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var expansion = await context.Expansions.FirstOrDefaultAsync(x => x.Id == id);
        context.Expansions.Remove(expansion);
        await context.SaveChangesAsync(cancellationToken);
    }
}