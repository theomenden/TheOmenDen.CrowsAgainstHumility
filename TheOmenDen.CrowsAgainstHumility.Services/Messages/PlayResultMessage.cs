using TheOmenDen.CrowsAgainstHumility.Services.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.Messages;
public sealed class PlayResultMessage: Message
{
    public IEnumerable<PlayResultItem> RoundResult { get; set; } = Enumerable.Empty<PlayResultItem>();
}
