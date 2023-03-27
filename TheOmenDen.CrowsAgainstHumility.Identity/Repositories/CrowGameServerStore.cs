using System.Collections;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Identity.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Repositories;
public class CrowGameServerStore : ICrowGameServerStore
{
    private readonly IDbContextFactory<AuthDbContext> _contextFactory;
    private readonly ILogger<CrowGameServerStore> _logger;

    public CrowGameServerStore(IDbContextFactory<AuthDbContext> contextFactory, ILogger<CrowGameServerStore> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    #region Synchronous Methods
    public CrowGameServer Create(IList<Guid> packs)
    {
        using var context = _contextFactory.CreateDbContext();

        var server = new CrowGameServer();

        context.GameServers.Add(server);
        context.SaveChanges();
        return server;
    }

    public CrowGameServer GetById(Guid id)
    {
        using var context = _contextFactory.CreateDbContext();

        var server = context.GameServers.FirstOrDefault(s => s.Id == id);

        return server;
    }

    public CrowGameServer GetByLobbyCode(string lobbyCode)
    {
        using var context = _contextFactory.CreateDbContext();

        var server = context.GameServers.FirstOrDefault(s => s.LobbyCode == lobbyCode);

        return server;
    }

    public IEnumerable<CrowGameServer> GetAll()
    {
        using var context = _contextFactory.CreateDbContext();

        var servers = context.GameServers.ToArray();

        return servers;
    }

    public void Remove(CrowGameServer server)
    {
        using var context = _contextFactory.CreateDbContext();

        context.GameServers.Remove(server);
        context.SaveChanges();
    }
    #endregion
    #region Asynchronous Methods
    public async Task<CrowGameServer> CreateAsync(IList<WhiteCard> whiteCards, IList<BlackCard> blackCards, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var server = new CrowGameServer();

        await context.GameServers.AddAsync(server, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return server;
    }

    public async Task<CrowGameServer> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var server = await context.GameServers.FirstOrDefaultAsync(server => server.Id == id, cancellationToken);

        return server;
    }

    public async Task<CrowGameServer> GetByLobbyCodeAsync(string lobbyCode, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var server = await context.GameServers
            .FirstOrDefaultAsync(server => server.LobbyCode == lobbyCode, cancellationToken);

        return server;
    }

    public async Task<IEnumerable<CrowGameServer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var servers = await context.GameServers.ToListAsync(cancellationToken);

        return servers;
    }

    public async Task RemoveAsync(CrowGameServer server, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        context.GameServers.Remove(server);
        context.SaveChangesAsync(cancellationToken);
    }
    #endregion

    public IEnumerator<CrowGameServer> GetEnumerator()
    {
        var enumerator = GetAll().GetEnumerator();

        return enumerator;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public async IAsyncEnumerator<CrowGameServer> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return context.GameServers.GetAsyncEnumerator(cancellationToken);
    }
}
