using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging.Messages;
public sealed class LobbyMemberWhiteCardMessage: LobbyMessage
{
    public LobbyMemberWhiteCardMessage() {}
    public LobbyMemberWhiteCardMessage(string lobbyCode, MessageTypes messageType) : base(lobbyCode, messageType) {}

    public string MemberName { get; set; } = String.Empty;
    public WhiteCard? PlayedCard { get; set; }
}
