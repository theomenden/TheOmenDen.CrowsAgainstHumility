namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed record CrowGameDto(IEnumerable<Pack> Packs, IEnumerable<Player> Players, String Name, String LobbyCode, Guid Id);
