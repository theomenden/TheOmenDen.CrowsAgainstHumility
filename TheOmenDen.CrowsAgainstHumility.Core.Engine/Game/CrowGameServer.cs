using TheOmenDen.CrowsAgainstHumility.Core.Engine.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;

public sealed record CrowGameServer(
    Guid Id,
    string Name,
    string Code,
    IDictionary<string, Player> Players,
    CrowGameSession CurrentSession,
    DateTime CreatedAt
    );
