using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.Messages;
public class WhiteCardChosenMessage: Message
{
    public IEnumerable<WhiteCard> WhiteCards { get; set; } = Enumerable.Empty<WhiteCard>();
}
