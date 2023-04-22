using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging.Messages;
public sealed class LobbyMemberMessage: LobbyMessage
{
    public LobbyMemberMessage() {}
    public LobbyMemberMessage(string lobbyCode, MessageTypes messageType): base(lobbyCode, messageType) {}

    public string MemberName { get; set; } = String.Empty;
    public string MemberType { get; set; } = String.Empty;
    public Guid SessionId { get; set; }
    public long AcknowledgedMessageId { get; set; }
}
