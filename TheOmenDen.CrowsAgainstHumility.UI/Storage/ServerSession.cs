using TheOmenDen.CrowsAgainstHumility.Core.Engine.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Storage;

public record ServerSession(Guid ServerId, string? Username, Guid RecoveryId, GameRoles Role);
