using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels.Game;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;
public sealed class CrowGameServerViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<PlayerViewModel>? Players { get; set; }
    public CrowGameSessionViewModel? CurrentSession { get; set; }
}
