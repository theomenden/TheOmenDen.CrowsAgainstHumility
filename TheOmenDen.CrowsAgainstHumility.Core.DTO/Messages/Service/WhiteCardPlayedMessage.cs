using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Messages.Service;
public sealed class WhiteCardPlayedMessage : Message
{
    public IEnumerable<WhiteCard> WhiteCards { get; set; } = Enumerable.Empty<WhiteCard>();
}
