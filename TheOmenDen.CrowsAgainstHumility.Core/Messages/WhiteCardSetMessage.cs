using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Serialization;

namespace TheOmenDen.CrowsAgainstHumility.Core.Messages;
public class WhiteCardSetMessage : Message
{
    public WhiteCardSetMessage(MessageTypes type, IEnumerable<WhiteCard> whiteCards)
        : base(type) => WhiteCards = whiteCards ?? throw new ArgumentNullException(nameof(whiteCards));

    internal WhiteCardSetMessage(Serialization.MessageData messageData)
        : base(messageData) => WhiteCards = messageData.WhiteCards?.ToList() ?? throw new ArgumentNullException(nameof(messageData));

    public IEnumerable<WhiteCard> WhiteCards { get; }

    protected internal override MessageData GetData()
    {
        var result = base.GetData();
        result.WhiteCards = WhiteCards.ToList();
        return result;
    }
}
