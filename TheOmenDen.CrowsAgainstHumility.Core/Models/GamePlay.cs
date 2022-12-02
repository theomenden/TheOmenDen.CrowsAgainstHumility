namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class GamePlay
{
    public string HubConnectionId { get; set; }

    public CrowGame Game { get; set; }

    public Player Player { get; set; }

    public WhiteCard? PlayedCard { get; set; }

    public bool HasPlayed => PlayedCard is not null;
}
