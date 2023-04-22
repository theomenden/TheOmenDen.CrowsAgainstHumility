
namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Events;

public sealed record LogUpdatedEventArgs(Guid ServerId, String InitiatingPlayer, String LogMessage) : CrowGameEventArgs(ServerId);
