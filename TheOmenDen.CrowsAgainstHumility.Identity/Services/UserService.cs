using TheOmenDen.CrowsAgainstHumility.Core.Auth.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Criteria;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Results;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Services;

internal sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApplicationUser> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _userRepository.GetUserAsync(userId, cancellationToken);

    public async Task<SearchResult<UserViewModel>> GetUsersAsync(SearchUsersCriteria criteria,
        CancellationToken cancellationToken = default)
        => await _userRepository.GetUsersAsync(criteria, cancellationToken);

    public ApplicationUser GetUser(Guid userId)
        => _userRepository.GetUser(userId);

    public async ValueTask<UserViewModel> GetUserViewModelAsync(Guid userId,
        CancellationToken cancellationToken = default)
        => await _userRepository.GetUserViewModelAsync(userId, cancellationToken);

    public async Task<ApplicationUser?> GetUserByUsernameAsync(string username,
            CancellationToken cancellationToken = default)
            => await _userRepository.GetUserByUsernameAsync(username, cancellationToken);

    public async ValueTask<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    => await _userRepository.IsInRoleAsync(userId, cancellationToken, role);

    public async ValueTask<bool> IsInRoleAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken = default)
        => await _userRepository.IsInRoleAsync(userId, cancellationToken, roles.ToArray());

    public async ValueTask<bool> IsInRoleAsync(Guid userId, CancellationToken cancellationToken = default, params string[] roles)
        => await _userRepository.IsInRoleAsync(userId, cancellationToken, roles);

    public async ValueTask<bool> IsExistingUserAsync(string username, CancellationToken cancellationToken = default)
    => await _userRepository.IsExistingUserAsync(username, cancellationToken);
}
