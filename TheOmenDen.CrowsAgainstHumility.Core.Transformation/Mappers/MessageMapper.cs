using Riok.Mapperly.Abstractions;

namespace TheOmenDen.CrowsAgainstHumility.Core.Transformation.Mappers;
[Mapper]
public partial class MessageMapper
{
    public partial DTO.Messages.Service.Message ToServiceMessage(DTO.Messages.Domain.Message message);
}
