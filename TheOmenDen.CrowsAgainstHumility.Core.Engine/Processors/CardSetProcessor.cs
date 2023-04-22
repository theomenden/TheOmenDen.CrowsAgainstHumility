using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
using TheOmenDen.Shared.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Processors;
internal static class CardSetProcessor
{
    internal static (bool wasParsed, Deck? parsedDeck, string? validationMessage) TryParseCardSet(IEnumerable<Pack> rawPacks)
    {
        var whiteCards = rawPacks.SelectMany(x => x.WhiteCards).ToList();
        var blackCards = rawPacks.SelectMany(x => x.BlackCards).ToList();

        if (!whiteCards.All(IsValidWhiteCard))
        {
            return (false, null, "One or more cards were invalid in your pack selection");
        }

        if (!blackCards.All(IsValidBlackCard))
        {
            return (false, null, "One or more cards were invalid in your pack selection");
        }

        return (true, new Deck(whiteCards.GetRandomElements(whiteCards.Count), blackCards.GetRandomElements(blackCards.Count)), null);
    }

    internal static bool IsValidWhiteCard(WhiteCard whiteCard)
    {
        return whiteCard.Id != Guid.Empty
            && !String.IsNullOrWhiteSpace(whiteCard.CardText);
    }

    internal static bool IsValidBlackCard(BlackCard blackCard)
    {
        return blackCard.Id != Guid.Empty
               && !String.IsNullOrWhiteSpace(blackCard.Message)
               && blackCard.PickAnswersCount >= 0;
    }
}
