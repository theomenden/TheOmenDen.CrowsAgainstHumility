using Riok.Mapperly.Abstractions;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Players;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Results.Service;

namespace TheOmenDen.CrowsAgainstHumility.Core.Transformation.Mappers;
[Mapper]
public partial class RoundResultItemMapper
{
    [MapProperty(nameof(KeyValuePair<Member, WhiteCardDto>.Key), nameof(RoundResultItem.Member))]
    [MapProperty(nameof(KeyValuePair<Member, WhiteCardDto>.Value), nameof(RoundResultItem.WhiteCard))]
    public partial RoundResultItem ToRoundResultItem(KeyValuePair<Member, WhiteCardDto> roundResultItem);

    private Member ValuePairToMember(KeyValuePair<Member, WhiteCardDto>  roundResultItem) => roundResultItem.Key;
    private WhiteCardDto ValuePairToWhiteCard(KeyValuePair<Member, WhiteCardDto>  roundResultItem) => roundResultItem.Value;
}
