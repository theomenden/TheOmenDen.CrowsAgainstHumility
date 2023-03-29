using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Serialization;
public class MessageData
{
    public long Id { get; set; }
    public MessageTypes MessageType { get; set; }
    public string? PlayerName { get; set; }
    public IDictionary<string, WhiteCard?> PlayedResult { get; set; }
    public IList<WhiteCard>? WhiteCards { get; set; }
    public DateTime? EndTime { get; set; }
}
