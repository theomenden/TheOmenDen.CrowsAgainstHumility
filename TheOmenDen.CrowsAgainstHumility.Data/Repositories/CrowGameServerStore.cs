using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Data.Repositories;
public class CrowGameServerStore: ICrowGameServerStore
{
    private readonly IDbContextFactory<CrowsAgainstHumilityContext> _contextFactory;
    private readonly ILogger<CrowGameServerStore> _logger;

    public CrowGameServer Create(IList<WhiteCard> whiteCards, IList<BlackCard> blackCards)
    {
        throw new NotImplementedException();
    }

    public CrowGameServer GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public CrowGameServer GetByLobbyCode(string lobbyCode)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CrowGameServer> GetAll()
    {
        throw new NotImplementedException();
    }

    public void Remove(CrowGameServer server)
    {
        throw new NotImplementedException();
    }

    public async Task<CrowGameServer> CreateAsync(IList<WhiteCard> whiteCards, IList<BlackCard> blackCards, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CrowGameServer> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CrowGameServer> GetByLobbyCodeAsync(string lobbyCode, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CrowGameServer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(CrowGameServer server, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
