using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Serialization;
public class PlayerData
{
    public string Name { get; set; } = String.Empty;
    public GameRoles MemberType { get; set; }
    public IList<MessageData> Messages { get; set; } = new List<MessageData>();
    public long LastMessageId { get; set; }
    public Guid SessionId { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsDormant { get; set; }
    public WhiteCard? WhiteCard { get; set; }
}
