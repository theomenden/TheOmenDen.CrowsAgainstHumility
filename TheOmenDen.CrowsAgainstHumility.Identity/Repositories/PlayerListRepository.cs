using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Identity.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Repositories;
internal sealed class PlayerListRepository
{
    private readonly IDbContextFactory<AuthDbContext> _contextFactory;
    private readonly ILogger<PlayerListRepository> _logger;

    public PlayerListRepository(IDbContextFactory<AuthDbContext> contextFactory, ILogger<PlayerListRepository> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }
}
