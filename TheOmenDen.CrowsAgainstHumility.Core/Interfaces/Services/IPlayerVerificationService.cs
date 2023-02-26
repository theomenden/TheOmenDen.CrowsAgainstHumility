using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IPlayerVerificationService
{
    Boolean IsPlayerInGameList(IEnumerable<String> playersInGame, String playerToVerify);

    Task<User?> CheckTwitchForUserAsync(string username);

    ValueTask<String> GetProfileImageUrlAsync(string username, CancellationToken cancellationToken = default);

    IAsyncEnumerable<User> CheckTwitchForUsersAsync(IEnumerable<string> usernames, CancellationToken cancellationToken = default);
}