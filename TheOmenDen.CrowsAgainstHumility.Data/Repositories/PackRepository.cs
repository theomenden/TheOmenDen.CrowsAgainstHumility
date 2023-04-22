using System.Collections;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.Shared.Specifications;

namespace TheOmenDen.CrowsAgainstHumility.Data.Repositories;
internal sealed class PackRepository : IPackRepository
{
    #region Injected Members
    private readonly IDbContextFactory<CrowsAgainstHumilityContext> _dbContextFactory;
    private readonly ILogger<PackRepository> _logger;
    #endregion
    #region Constructors
    public PackRepository(IDbContextFactory<CrowsAgainstHumilityContext> dbContextFactory, ILogger<PackRepository> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }
    #endregion
    #region IAccessor Implementations
    public async Task<IEnumerable<Pack>> GetAllAsync(CancellationToken cancellationToken = new())
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var whiteCards = await GetFilteredWhiteCards(context, cancellationToken);

        var blackCards = await GetFilteredBlackCards(context, cancellationToken);

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

    public Task<IEnumerable<Pack>> GetAllThatMatchAsync(Expression<Func<Pack, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Pack>> GetAllThatMatchAsync(Specification<Pack> specification, CancellationToken cancellationToken = new CancellationToken())
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var packsQuery = context.Packs.Where(specification.ToExpression())
            .Select(p => new Pack
            {
                IsOfficialPack = p.IsOfficialPack,
                Name = p.Name,
                Description = p.Description,
                Id = p.Id
            });

        var whiteCardQuery = from whiteCard in context.vw_FilteredWhiteCardsByPack
                             join pack in packsQuery
                                 on whiteCard.PackId equals pack.Id
                             select new WhiteCard
                             {
                                 CardText = whiteCard.CardText,
                                 Id = whiteCard.Id,
                                 PackId = whiteCard.PackId
                             };

        var blackCardQuery = from blackCard in context.vw_FilteredBlackCardsByPack
                             join pack in packsQuery
                                 on blackCard.PackId equals pack.Id
                             select new BlackCard
                             {
                                 PickAnswersCount = blackCard.PickAnswersCount,
                                 Id = blackCard.Id,
                                 PackId = blackCard.PackId,
                                 Message = blackCard.Message
                             };

        return from pack in packsQuery
               join whiteCard in whiteCardQuery
                   on pack.Id equals whiteCard.PackId
                   into gwc
               join blackCard in blackCardQuery
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
    #endregion
    #region IAsyncStreamAccessor Implementation
    public async IAsyncEnumerable<Pack> GetAllAsAsyncStream([EnumeratorCancellation] CancellationToken cancellationToken = new CancellationToken())
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var filteredWhiteCards = await GetFilteredWhiteCards(context, cancellationToken);

        var filteredBlackCards = await GetFilteredBlackCards(context, cancellationToken);

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

    public IAsyncEnumerable<Pack> GetAllThatMatchAsAsyncStream(Expression<Func<Pack, bool>> predicate,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<Pack> GetAllThatMatchAsAsyncStream(Specification<Pack> specification,
        [EnumeratorCancellation] CancellationToken cancellationToken = new CancellationToken())
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var packsQuery = context.Packs.Where(specification.ToExpression())
            .Select(p => new Pack
            {
                IsOfficialPack = p.IsOfficialPack,
                Name = p.Name,
                Description = p.Description,
                Id = p.Id
            });

        var whiteCardQuery = from whiteCard in context.vw_FilteredWhiteCardsByPack
                             join pack in packsQuery
                                 on whiteCard.PackId equals pack.Id
                             select new WhiteCard
                             {
                                 CardText = whiteCard.CardText,
                                 Id = whiteCard.Id,
                                 PackId = whiteCard.PackId
                             };

        var blackCardQuery = from blackCard in context.vw_FilteredBlackCardsByPack
                             join pack in packsQuery
                                 on blackCard.PackId equals pack.Id
                             select new BlackCard
                             {
                                 PickAnswersCount = blackCard.PickAnswersCount,
                                 Id = blackCard.Id,
                                 PackId = blackCard.PackId,
                                 Message = blackCard.Message
                             };

        var finalQuery = from pack in packsQuery
                         join whiteCard in whiteCardQuery
                             on pack.Id equals whiteCard.PackId
                             into gwc
                         join blackCard in blackCardQuery
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

        await foreach (var resolvedPack in finalQuery.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            yield return resolvedPack;
        }
    }
    #endregion
    #region IAsyncEnumerable Implementation
    public async IAsyncEnumerator<Pack> GetAsyncEnumerator(CancellationToken cancellationToken = new())
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var blackCards = await GetFilteredBlackCards(context, cancellationToken);
        var whiteCards = await GetFilteredWhiteCards(context, cancellationToken);

        await foreach (var pack in context.Packs
                           .AsAsyncEnumerable()
                           .Select(p => new Pack
                           {
                               BlackCards = blackCards.Where(bc => bc.PackId == p.Id),
                               WhiteCards = whiteCards.Where(wc => wc.PackId == p.Id),
                               Description = p.Description,
                               Id = p.Id,
                               IsOfficialPack = p.IsOfficialPack,
                               Name = p.Name,
                           })
                           .WithCancellation(cancellationToken))
        {
            yield return pack;
        }
    }
    #endregion
    #region IEnumerable Implementation
    public IEnumerator<Pack> GetEnumerator()
    {
        using var context = _dbContextFactory.CreateDbContext();

        var query = context.Packs.ToList();

        return query.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion
    #region Private Methods
    private static async Task<BlackCard[]> GetFilteredBlackCards(CrowsAgainstHumilityContext context, CancellationToken cancellationToken) => await context.vw_FilteredBlackCardsByPack
            .Select(blackCard => new BlackCard
            {
                PickAnswersCount = blackCard.PickAnswersCount,
                Id = blackCard.Id,
                PackId = blackCard.PackId,
                Message = blackCard.Message
            })
            .ToArrayAsync(cancellationToken);

    private static async Task<WhiteCard[]> GetFilteredWhiteCards(CrowsAgainstHumilityContext context, CancellationToken cancellationToken) =>
        await context.vw_FilteredWhiteCardsByPack
            .Select(whiteCard => new WhiteCard
            {
                CardText = whiteCard.CardText,
                Id = whiteCard.Id,
                PackId = whiteCard.PackId
            })
            .ToArrayAsync(cancellationToken);

    #endregion
}
