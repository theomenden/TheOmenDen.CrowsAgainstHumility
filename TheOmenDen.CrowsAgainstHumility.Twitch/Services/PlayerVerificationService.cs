using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace TheOmenDen.CrowsAgainstHumility.Twitch.Services;

/// <summary>
/// <para>This service aims to reach out to the Twitch API and hit the Users Endpoint</para>
/// <para>The idea here is to verify that a username, exists and is valid.</para>
/// <para>Then store it underneath a CrowGame, to make sure that only users in the game can join or rejoin.</para>
/// </summary>
internal sealed class PlayerVerificationService : IPlayerVerificationService
{
    private readonly TwitchAPI? _twitchApi;

    private readonly ILogger<PlayerVerificationService> _logger;

    public PlayerVerificationService(TwitchAPI twitchApi, ILogger<PlayerVerificationService> logger)
    {
        _logger = logger;
        _twitchApi = twitchApi;
    }

    public async ValueTask<String> GetProfileImageUrlAsync(string username,
        CancellationToken cancellationToken = default)
    {
        await GenerateAccessTokenAsync();

        var userAccount = await _twitchApi!.Helix.Users.GetUsersAsync(null, new List<String> { username });

        return userAccount.Users[0].ProfileImageUrl;
    }

    public Boolean IsPlayerInGameList(IEnumerable<String> playersInGame, String playerToVerify)
        => playersInGame.Contains(playerToVerify, StringComparer.OrdinalIgnoreCase);

    public async Task<User?> CheckTwitchForUserAsync(String username)
    {
        await GenerateAccessTokenAsync();

        User? user;

        try
        {
            var userResponse = await _twitchApi!.Helix.Users.GetUsersAsync(null, new() { username });

            user = userResponse.Users[0];
        }
        catch
        {
            _logger.LogWarning("{Username} was not a valid twitch handle", username);
            user = null;
        }

        return user;
    }

    public async IAsyncEnumerable<User> CheckTwitchForUsersAsync(IEnumerable<String> usernames,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            yield break;
        }

        await GenerateAccessTokenAsync();

        var userResponse = await _twitchApi!.Helix.Users.GetUsersAsync(null, usernames.ToList());

        foreach (var user in userResponse.Users)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            yield return user;
        }
    }

    private async Task GenerateAccessTokenAsync()
    {
        if (!String.IsNullOrWhiteSpace(_twitchApi!.Settings.AccessToken))
        {
            return;
        }

        var accessToken = await _twitchApi!.Auth.GetAccessTokenAsync();

        _twitchApi.Settings.AccessToken = accessToken;
    }
}
