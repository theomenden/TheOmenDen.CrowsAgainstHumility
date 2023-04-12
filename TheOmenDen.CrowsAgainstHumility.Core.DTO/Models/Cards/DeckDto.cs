namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
public sealed record DeckDto(Guid LobbyId, IEnumerable<PackDto> Packs);