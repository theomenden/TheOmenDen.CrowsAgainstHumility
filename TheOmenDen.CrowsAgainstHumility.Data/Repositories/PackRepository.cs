using System.Collections;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.Shared.Specifications;
using TwitchLib.EventSub.Core.Models.ChannelGoals;

namespace TheOmenDen.CrowsAgainstHumility.Data.Repositories;
internal sealed class PackRepository : IPackRepository
{
    private readonly IDbContextFactory<CrowsAgainstHumilityContext> _dbContextFactory;
    private readonly ILogger<PackRepository> _logger;

    public PackRepository(IDbContextFactory<CrowsAgainstHumilityContext> dbContextFactory, ILogger<PackRepository> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public IAsyncEnumerator<Pack> GetAsyncEnumerator(CancellationToken cancellationToken = new())
    => GetAllPacksAsyncStream(cancellationToken).GetAsyncEnumerator(cancellationToken);

    public async IAsyncEnumerable<Pack> GetAllPacksAsyncStream([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var filteredWhiteCards = context.vw_FilteredWhiteCardsByPack
            .Select(fwc => new WhiteCard
            {
                Id = fwc.Id,
                PackId = fwc.PackId,
                CardText = fwc.CardText,
            });

        var filteredBlackCards = context.vw_FilteredBlackCardsByPack
            .Select(fbc => new BlackCard
            {
                Id = fbc.Id,
                PackId = fbc.PackId,
                Message = fbc.Message,
                PickAnswersCount = fbc.PickAnswersCount
            });

        await foreach (var pack in context.Packs
                           .Select(p => new
                           {
                               p.Id,
                               p.Name,
                               p.Description
                           })
                           .AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            yield return new Pack
            {
                Id = pack.Id,
                Description = pack.Description,
                Name = pack.Name,
                BlackCards = filteredBlackCards ?? Enumerable.Empty<BlackCard>(),
                WhiteCards = filteredWhiteCards ?? Enumerable.Empty<WhiteCard>()
            };
        }
    }

    public async Task<IEnumerable<Pack>> GetAllPacksAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var whiteCards = await context.vw_FilteredWhiteCardsByPack
            .Select(whiteCard => new WhiteCard
            {
                CardText = whiteCard.CardText,
                Id = whiteCard.Id,
                PackId = whiteCard.PackId
            }).ToArrayAsync(cancellationToken);

        var blackCards = await context.vw_FilteredBlackCardsByPack
            .Select(blackCard => new BlackCard
            {
                PickAnswersCount = blackCard.PickAnswersCount,
                Id = blackCard.Id,
                PackId = blackCard.PackId,
                Message = blackCard.Message
            })
            .ToArrayAsync(cancellationToken);

        var packs = await context.Packs.ToListAsync(cancellationToken);

        return from pack in packs
               join whiteCard in whiteCards
                   on pack.Id equals whiteCard.PackId 
                   into gwc
               join blackCard in blackCards
                   on pack.Id equals blackCard.PackId
                   into gbc
               select new Pack
               {
                   Id = pack.Id,
                   IsOfficialPack = pack.IsOfficialPack,
                   Name = pack.Name,
                   Description = pack.Description,
                   BlackCards = gbc,
                   WhiteCards = gwc,
               };
    }

    public async IAsyncEnumerable<Pack> GetAllPacksThatMatch(Specification<Pack> specification, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = context.Packs
            .AsNoTracking()
            .Include(p => p.WhiteCards)
            .Include(p => p.BlackCards)
            .Where(specification.ToExpression());

        await foreach (var pack in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            yield return pack;
        }
    }
}
