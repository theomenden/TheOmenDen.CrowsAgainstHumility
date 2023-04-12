namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
public sealed record PackDto(String Name, IEnumerable<WhiteCardDto> WhiteCards, IEnumerable<BlackCardDto> BlackCards);
