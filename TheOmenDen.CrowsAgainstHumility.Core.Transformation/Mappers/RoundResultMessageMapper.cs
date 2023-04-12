using Riok.Mapperly.Abstractions;

namespace TheOmenDen.CrowsAgainstHumility.Core.Transformation.Mappers;
[Mapper]
public partial class RoundResultMessageMapper
{
    public partial DTO.Messages.Service.RoundResultMessage ToServiceRoundResultMessage(DTO.Messages.Domain.RoundResultMessage message);
}
