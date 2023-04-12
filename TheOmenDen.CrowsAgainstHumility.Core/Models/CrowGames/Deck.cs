namespace TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
public sealed record Deck(String LobbyCode, IEnumerable<WhiteCard> WhiteCards, IEnumerable<BlackCard> BlackCards);