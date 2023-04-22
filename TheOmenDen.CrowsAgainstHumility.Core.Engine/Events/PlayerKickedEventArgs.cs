using TheOmenDen.CrowsAgainstHumility.Core.Engine.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Events;

public sealed record PlayerKickedEventArgs(Guid ServerId, Player KickedPlayer) : CrowGameEventArgs(ServerId);
