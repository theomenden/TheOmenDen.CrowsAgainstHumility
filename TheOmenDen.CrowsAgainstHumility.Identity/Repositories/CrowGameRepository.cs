using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Identity.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Repositories;
internal sealed class CrowGameRepository
{
    private readonly IDbContextFactory<AuthDbContext> _dbContextFactory;
    private readonly ILogger<CrowGameRepository> _logger;

    public CrowGameRepository(IDbContextFactory<AuthDbContext> dbContextFactory, ILogger<CrowGameRepository> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;   
    }

    public async Task<CrowGame> AddGrowGameAsync(CrowGame game, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await context.Games.AddAsync(game, cancellationToken);
        await context.AddAsync(cancellationToken, cancellationToken);

        return game;
    }
}
