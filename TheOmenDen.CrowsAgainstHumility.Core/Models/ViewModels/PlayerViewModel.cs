using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;
public sealed class PlayerViewModel
{
    public string? ConnectionId { get; set; }
    public Guid? RecoveryId { get; set; }
    public int PublicId { get; set; }
    public string? Username { get; set; }
    public GameRole Role { get; set; } = GameRole.Observer;
    public PlayerMode Mode { get; set; } = PlayerMode.Awake;
}
