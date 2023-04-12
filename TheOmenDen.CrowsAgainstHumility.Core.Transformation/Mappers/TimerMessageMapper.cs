using Riok.Mapperly.Abstractions;

namespace TheOmenDen.CrowsAgainstHumility.Core.Transformation.Mappers;
[Mapper]
public partial class TimerMessageMappers
{
    public partial DTO.Messages.Service.TimerMessage ToServiceTimerMessage(DTO.Messages.Domain.TimerMessage domainMessage);
}
