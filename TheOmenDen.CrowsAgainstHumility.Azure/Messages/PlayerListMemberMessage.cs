using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Messages;
public sealed class PlayerListMemberMessage: PlayerListMessage
{
    public PlayerListMemberMessage() {}

    public PlayerListMemberMessage(string playerListName, MessageTypes messageType) : base(playerListName, messageType) {}

    public string MemberName { get; set; } = String.Empty;
    public GameRoles GameRole { get; set; } = GameRoles.Player;
    public Guid SessionId { get; set; }
    public long AcknowledgedMessageId { get; set; }
}
