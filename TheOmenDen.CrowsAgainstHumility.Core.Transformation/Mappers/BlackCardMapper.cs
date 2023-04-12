using Riok.Mapperly.Abstractions;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Transformation.Mappers;
[Mapper]
public partial class BlackCardMapper
{
    [MapProperty(nameof(BlackCard.Id), nameof(BlackCardDto.Id))]
    [MapProperty(nameof(BlackCard.Message), nameof(BlackCardDto.Text))]
    [MapProperty(nameof(BlackCard.PickAnswersCount), nameof(BlackCardDto.Answers))]
    public partial BlackCardDto MapToBlackCardDto(BlackCard blackCard);
    public partial IEnumerable<BlackCardDto> MapFromBlackCards(IEnumerable<BlackCard> blackCard);
}
