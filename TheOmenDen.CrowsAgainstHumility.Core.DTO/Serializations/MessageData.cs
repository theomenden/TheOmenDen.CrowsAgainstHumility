using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Serializations;
public class MessageData
{
    public long Id { get; set; }
    public MessageTypes MessageType { get; set; }
    public string? MemberName { get; set; }
    public IDictionary<string, WhiteCardDto?>? PlayedCardResult { get; set; }
    public IEnumerable<WhiteCardDto>? WhiteCards { get; set; } = Enumerable.Empty<WhiteCardDto>();
    public DateTime? EndedAt { get; set; }
}
