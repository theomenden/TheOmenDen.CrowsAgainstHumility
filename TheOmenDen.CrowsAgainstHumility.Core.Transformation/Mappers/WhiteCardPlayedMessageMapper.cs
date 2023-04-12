using Riok.Mapperly.Abstractions;

namespace TheOmenDen.CrowsAgainstHumility.Core.Transformation.Mappers;
[Mapper]
public partial class WhiteCardPlayedMessageMapper
{
    public partial DTO.Messages.Service.WhiteCardPlayedMessage ToServiceWhiteCardPlayedMessage(
        DTO.Messages.Domain.WhiteCardPlayedMessage message);
}
