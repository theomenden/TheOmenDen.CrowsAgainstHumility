using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Serializations;
public class MemberData
{
    public string Name { get; set; } = String.Empty;
    public GameRoles GameRole { get; set; } = GameRoles.Observer;
    public IEnumerable<MessageData> Messages { get; set; } = Enumerable.Empty<MessageData>();
    public long LastMessageId { get; set; }
    public Guid SessionId { get; set; }
    public DateTime LastActiveAt { get; set; }
    public bool IsDormant { get; set; }
    public WhiteCardDto? PlayedCard { get; set; }
}
