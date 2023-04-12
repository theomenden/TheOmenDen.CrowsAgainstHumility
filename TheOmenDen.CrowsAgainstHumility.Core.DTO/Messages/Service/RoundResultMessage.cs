using TheOmenDen.CrowsAgainstHumility.Core.DTO.Results.Service;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Messages.Service;
public sealed class RoundResultMessage : Message
{
    public IEnumerable<RoundResultItem> RoundResult { get; set; } = Enumerable.Empty<RoundResultItem>();
}
