namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Events;

public sealed record RoomClearedEventArgs(Guid ServerId) : CrowGameEventArgs(ServerId);
