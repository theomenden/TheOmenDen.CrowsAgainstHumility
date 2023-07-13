using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;
using TheOmenDen.Shared.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Hands;

public class Deck : ICardProvider
{
    private readonly List<BlackCard> _blackCards = new();
    private readonly List<WhiteCard> _whiteCards = new();

    private IPackRepository _packRepository;

    public Deck(IPackRepository packRepository)
    {
        _packRepository = packRepository;
    }

    public BlackCard DrawBlackCard()
    {
        return _blackCards.First();
    }

    public ICollection<WhiteCard> DrawWhiteCards(int numberOfCards)
    {
        return _whiteCards.GetRandomElements(numberOfCards).ToList();
    }
}