using System.Collections.Concurrent;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
public sealed class CrowGameServer
{
    public CrowGameServer(Guid id, Deck deck)
    {
        Id = id;
        Players = new ConcurrentDictionary<string, Observer>();
        CurrentSession = new (deck);
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; set; }
    public String Name { get; set; } = String.Empty;
    public String LobbyCode { get; set; } = String.Empty;
    public IDictionary<string, Observer> Players { get; set; }
    public CrowGame CurrentSession { get; set; }
    public DateTime CreatedAt { get; set; }
}
