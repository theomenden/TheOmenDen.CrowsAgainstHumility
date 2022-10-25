namespace TheOmenDen.CrowsAgainstHumility.Core.Models;

public class CrowGamePlayer
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public virtual ApplicationUser Player { get; set; }

    public Guid GameId { get; set; }

    public virtual CrowGame Game { get; set; }
}
