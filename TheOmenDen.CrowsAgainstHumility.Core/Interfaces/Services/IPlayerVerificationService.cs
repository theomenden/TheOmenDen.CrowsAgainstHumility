using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IPlayerVerificationService
{
    Boolean IsPlayerInGameList(IEnumerable<String> playersInGame, String playerToVerify);

    Task<User?> CheckTwitchForUser(string username);

    IAsyncEnumerable<User> CheckTwitchForUsers(IEnumerable<string> usernames, CancellationToken cancellationToken = default);
}