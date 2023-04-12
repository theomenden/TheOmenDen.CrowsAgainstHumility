using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Results;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Structs;
public ref struct MessageData
{
    public long Id { get; set; }
    public MessageTypes Type { get; set; }
    public Player Player { get; set; }
    public IList<RoundResult>? RoundResult { get; set; }
    public IList<WhiteCard>? WhiteCards { get; set; }
    public DateTime EndTimer { get; set; }
}
