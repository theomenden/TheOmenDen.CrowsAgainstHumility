namespace TheOmenDen.CrowsAgainstHumility.Services;

public sealed class CrowGameService
{
    public Player Player { get; set; } = new();

    public CrowGame Game { get; set; } = new();

    public bool IsStateReady => !(String.IsNullOrWhiteSpace(Player.Username) 
                                  || String.IsNullOrWhiteSpace(Game.Name));
}