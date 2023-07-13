using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public sealed record CrowGamePlayer(Guid Id, List<WhiteCard> Hand, WhiteCard PlayedCard);