using Riok.Mapperly.Abstractions;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Transformation.Mappers;
[Mapper]
public partial class WhiteCardMapper
{
    [MapProperty(nameof(WhiteCard.CardText), nameof(WhiteCardDto.Text))]
    [MapProperty(nameof(WhiteCard.Id), nameof(WhiteCardDto.Id))]
    public partial WhiteCardDto MapToWhiteCardDto(WhiteCard whiteCard);

    public partial IEnumerable<WhiteCardDto> MapFromWhiteCards(IEnumerable<WhiteCard> whiteCard);
}
