using TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public class GameContext
{
    public Deck<BlackCard> BlackDeck { get; set; }
    public Deck<WhiteCard> WhiteDeck { get; set; }



}