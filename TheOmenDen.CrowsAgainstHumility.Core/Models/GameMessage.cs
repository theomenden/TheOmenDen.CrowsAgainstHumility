using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed record GameMessage(MessageTypes MessageType, String? Message, String? PlayerName, DateTime CreatedAt)
{
    public Guid Id => Guid.NewGuid();
}
