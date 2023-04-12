using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Messages;
public sealed class PlayerListWhiteCardsSetMessage: PlayerListMessage
{
    public PlayerListWhiteCardsSetMessage() {}

    public PlayerListWhiteCardsSetMessage(string playerListName, MessageTypes messageType) : base(playerListName, messageType) {}

    public IList<WhiteCard?> WhiteCards { get; set; } = new List<WhiteCard?>();
}
