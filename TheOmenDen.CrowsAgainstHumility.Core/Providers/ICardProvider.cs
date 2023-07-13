using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Providers;

public interface ICardProvider
{
    BlackCard DrawBlackCard();
    ICollection<WhiteCard> DrawWhiteCards(int numberOfCards);
}