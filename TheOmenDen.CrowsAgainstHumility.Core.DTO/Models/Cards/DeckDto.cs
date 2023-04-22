using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
public sealed record Deck(IEnumerable<WhiteCard> WhiteCards, IEnumerable<BlackCard> BlackCards);