using TheOmenDen.CrowsAgainstHumility.Core.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public class GameContext
{
    public Deck<ImmutableBlackCard> BlackDeck { get; set; }
    public Deck<ImmutableWhiteCard> WhiteDeck { get; set; }

}