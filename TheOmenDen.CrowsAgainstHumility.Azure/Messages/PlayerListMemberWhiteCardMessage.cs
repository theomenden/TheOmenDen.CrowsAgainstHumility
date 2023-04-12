using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Messages;
public sealed class PlayerListMemberWhiteCardMessage: PlayerListMessage
{
    public PlayerListMemberWhiteCardMessage() {}

    public PlayerListMemberWhiteCardMessage(string playerListName, MessageTypes messageType) :base(playerListName, messageType) {}

    public string MemberName { get; set; } = String.Empty;
    public WhiteCard? WhiteCard { get; set; }
}
