using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Serializations;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Messages.Domain;
public sealed class WhiteCardPlayedMessage : Message
{
    #region Constructors
    public WhiteCardPlayedMessage(MessageTypes type, IEnumerable<WhiteCardDto> playedCards)
        : base(type)
    {
        ArgumentNullException.ThrowIfNull(playedCards);
        PlayedCards = playedCards;
    }

    internal WhiteCardPlayedMessage(MessageData messageData)
        : base(messageData)
    {
        ArgumentNullException.ThrowIfNull(messageData.WhiteCards);
        PlayedCards = messageData.WhiteCards.ToArray();
    }
    #endregion
    #region Properties
    public IEnumerable<WhiteCardDto> PlayedCards { get; }
    #endregion
    #region Overrides
    protected internal override MessageData GetData()
    {
        var result =  base.GetData();
        result.WhiteCards = PlayedCards.ToArray();
        return result;
    }
    #endregion
}
