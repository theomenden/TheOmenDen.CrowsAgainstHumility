using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed record CrowChatMessage(CrowChatMessageType MessageType, String? Message, String? PlayerName);
