using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.Shared.Extensions;
using TheOmenDen.Shared.Guards;

namespace TheOmenDen.CrowsAgainstHumility.Services.Decks;
internal static class DeckProcessor
{
    internal static (bool success, IEnumerable<WhiteCard> playableCards, IEnumerable<BlackCard> situationCards)
        TryParseCardSet(Deck deck)
    => !deck.WhiteCards.Any() || !deck.BlackCards.Any()
            ? (false, Enumerable.Empty<WhiteCard>(), Enumerable.Empty<BlackCard>())
            : (true, deck.WhiteCards, deck.BlackCards);
}
