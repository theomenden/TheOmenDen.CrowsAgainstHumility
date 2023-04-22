using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Messaging.Messages;
public sealed class LobbyWhiteCardPlayedMessage: LobbyMessage
{
    public LobbyWhiteCardPlayedMessage() {}
    public LobbyWhiteCardPlayedMessage(string lobbyCode, MessageTypes messageType): base(lobbyCode, messageType) {}

    public IEnumerable<WhiteCard?> WhiteCards { get; set; } = Enumerable.Empty<WhiteCard?>();
}
