using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Events;

public sealed record RoomUpdatedEventArgs(Guid ServerId, CrowGameServer UpdatedServer) : CrowGameEventArgs(ServerId);
