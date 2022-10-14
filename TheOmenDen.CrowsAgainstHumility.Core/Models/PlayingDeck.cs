namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class PlayingDeck
{
    public IEnumerable<WhiteCard> Cards { get; set; } = Enumerable.Empty<WhiteCard>();
}
