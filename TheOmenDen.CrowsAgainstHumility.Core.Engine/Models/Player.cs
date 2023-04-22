using TheOmenDen.CrowsAgainstHumility.Core.Engine.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Models;

public sealed record Player(
    String ConnectionId,
    Guid RecoveryId,
    int PublicId,
    String Username,
    GameRoles Type,
    PlayerMode Mode
);
