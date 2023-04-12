using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Serializations;
public class LobbyData
{
    public string Name { get; set; } = String.Empty;
    public LobbyState State { get; set; }
    public IEnumerable<WhiteCardDto> AvailableCards { get; set; } = Enumerable.Empty<WhiteCardDto>();
    public IEnumerable<BlackCardDto> AvailablePrompts { get; set; } = Enumerable.Empty<BlackCardDto>();
    public IEnumerable<MemberData> Members { get; set; } = Enumerable.Empty<MemberData>();
    public IDictionary<string, WhiteCardDto?>? RoundResult { get; set; }
    public DateTime? TimerEndedAt { get; set; }
}
