using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using Discord.WebSocket;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Client;

namespace TheOmenDen.CrowsAgainstHumility.Services.Authentication;

/// <summary>
/// <para>This service aims to reach out to the Twitch API and hit the Users Endpoint</para>
/// <para>The idea here is to verify that a username, exists and is valid.</para>
/// <para>Then store it underneath a CrowGame, to make sure that only users in the game can join or rejoin.</para>
/// </summary>
internal sealed class PlayerVerificationService : IPlayerVerificationService
{
    private readonly TwitchAPI? _twitchApi;

    private readonly DiscordSocketClient _discord;

    private readonly ILogger<PlayerVerificationService> _logger;

    public PlayerVerificationService(DiscordSocketClient discord, TwitchAPI twitchApi, ILogger<PlayerVerificationService> logger)
    {
        _logger = logger;
        _discord = discord;
        _twitchApi = twitchApi;
    }

    public Boolean IsPlayerInGameList(IEnumerable<String> playersInGame, String playerToVerify)
    => playersInGame.Contains(playerToVerify, StringComparer.OrdinalIgnoreCase);

    public Task<SocketUser?> CheckDiscordForUser(String username, String discriminator)
    {
        var socketUser = _discord.GetUser(username, discriminator);

        return Task.FromResult(socketUser ?? default);
    }

    public async Task<User?> CheckTwitchForUser(String username)
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

    public async IAsyncEnumerable<User> CheckTwitchForUsers(IEnumerable<String> usernames, [EnumeratorCancellation] CancellationToken cancellationToken = default)
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