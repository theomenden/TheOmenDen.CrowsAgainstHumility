using Riok.Mapperly.Abstractions;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Transformation.Mappers;
[Mapper]
public partial class PlayerMapper
{

    [MapProperty(nameof(ApplicationUser.Id), nameof(PlayerViewModel.RecoveryId))]
    [MapProperty(nameof(ApplicationUser.UserName), nameof(PlayerViewModel.Username))]
    public partial PlayerViewModel ApplicationUserToPlayer(ApplicationUser user);

    public partial IEnumerable<PlayerViewModel> ApplicationUsersToPlayers(IEnumerable<ApplicationUser> users);

    [MapperIgnoreSource(nameof(Player.ConnectionId))]
    [MapperIgnoreTarget(nameof(PlayerViewModel.ConnectionId))]
    public partial PlayerViewModel PlayerToPlayerViewModel(Player player);

    [MapProperty(nameof(Player.ConnectionId), nameof(PlayerViewModel.ConnectionId))]
    public partial PlayerViewModel PlayerToPlayerViewModelWithPrivateId(Player player);

    private PlayerMode EngineToDtoMode(Engine.Enumerations.PlayerMode mode) => PlayerMode.ParseFromValueOrDefault(mode.Value, PlayerMode.Awake);

}
