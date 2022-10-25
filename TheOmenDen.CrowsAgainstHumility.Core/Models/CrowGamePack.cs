namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public class CrowGamePack
{
    public Guid Id { get; set; }

    public Guid CrowGameId { get; set; }

    public Guid PackId { get; set; }

    public virtual CrowGame CrowGame { get; set; }
}
