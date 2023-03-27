using System.Collections.Concurrent;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
public sealed class CrowGameServer
{
    public CrowGameServer(Guid id, IList<Pack> packs)
    {
        Id = id;
        Players = new ConcurrentDictionary<string, Player>();
        CurrentSession = new CrowGame(packs);
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; set; }
    public Guid Name { get; set; }
    public Guid LobbyCode { get; set; }
    public IDictionary<string, Player> Players { get; set; }
    public CrowGame CurrentSession { get; set; }
    public DateTime CreatedAt { get; set; }
}
