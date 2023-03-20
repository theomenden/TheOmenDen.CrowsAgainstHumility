namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed record RoomOptions
{
    public IEnumerable<WhiteCard> WhiteCards { get; init; } = Enumerable.Empty<WhiteCard>();
    public IEnumerable<BlackCard> BlackCards { get; init; } = Enumerable.Empty<BlackCard>();
    public bool? WhiteCardsShow { get; init; }
    public bool? AutoShowWhiteCards { get; init; }
    public DateTime? WhiteCardPlayingTime { get; init; }
    public DateTime? CardTsarChoosingTime { get; init; }
}
