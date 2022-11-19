using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.Shared.Guards;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace TheOmenDen.CrowsAgainstHumility.Twitch.Services;

internal sealed class TwitchRedemptionsManagerService: ITwitchRedemptionsManager
{
    #region Fields
    private readonly ILogger<TwitchRedemptionsManagerService> _logger;
    private TwitchAPI? _twitchApi;
    private TwitchPubSub? _twitchPubSub;
    private Boolean _disposedValue;
    #endregion
    public TwitchRedemptionsManagerService(ILogger<TwitchRedemptionsManagerService> logger) => _logger = logger;
    public String Username { get; private set; } = String.Empty;
    #region Event Handlers
    public event EventHandler<OnChannelPointsRewardRedeemedArgs>? OnRewardRedeemed;
    #endregion
    #region Connection Methods
    public async Task ConnectToChannelAsync(String channel, String oauthToken, String username, String clientId, CancellationToken cancellationToken = new())
    {
        InitializeChannelConnections(channel, oauthToken, username, clientId);

        var channelId = await ConfigureApi(oauthToken, clientId, channel, cancellationToken);
        
        ConfigurePubSub(channelId, oauthToken);
    }
    private void InitializeChannelConnections(String channel, String oauthToken, String username, String clientId)
    {
        _logger.LogDebug("InitializeChannelConnection({Channel}, {Username})",
            channel, username);

        Guard.FromNullOrWhitespace(channel, nameof(channel));
        Guard.FromNullOrWhitespace(oauthToken, nameof(oauthToken));
        Guard.FromNullOrWhitespace(username, nameof(username));
        
        Username = username;
        _twitchApi    ??= new();
        _twitchPubSub ??= new();
    }
    #endregion
    #region Resource Cleanup
    public void DisconnectFromTwitch()
    {

        _logger.LogDebug("Disconnecting from Twitch");
        
        if (_twitchPubSub is not null)
        {
            _twitchPubSub.Disconnect();
            _twitchPubSub = null;
        }

        if (_twitchApi is not null)
        {
            _twitchApi = null;
        }
    }

    public ValueTask DisconnectFromTwitchAsync()
    {
        _logger.LogDebug("Disconnecting from Twitch");
        
        if (_twitchPubSub is not null)
        {
            _twitchPubSub.Disconnect();
            _twitchPubSub = null;
        }

        if (_twitchApi is not null)
        {
            _twitchApi = null;
        }

        return ValueTask.CompletedTask;
    }

    private void Dispose(Boolean disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            DisconnectFromTwitch();
        }

        _disposedValue = true;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync() => DisconnectFromTwitchAsync();
    #endregion
    #region Initializing Methods
    private async Task<String> ConfigureApi(String oauthToken, String clientId, String channel, CancellationToken cancellationToken = default)
    {
        _twitchApi ??= new();

        var twitchApiOauthToken = String.Empty;

        if (oauthToken.StartsWith("oath:"))
        {
            twitchApiOauthToken = oauthToken["oauth:".Length..];
        }

        _twitchApi.Settings.AccessToken = twitchApiOauthToken;
        _twitchApi.Settings.ClientId = clientId;

        try
        {
            _logger.LogDebug("[Twitch API] Trying to find channel ID...");

            var response = await _twitchApi.Helix.Users.GetUsersAsync(null, new() { channel });

            if (response.Users.Length is 1)
            {
                var channelId = response.Users[0].Id;
                _logger.LogDebug("[Twitch API] Channel ID for {Channel} is {ChannelId}", channel, channelId);

                return channelId;
            }

            throw new ArgumentException($"[Twitch API] Could not find Twitch user/channel {channel}");
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("@{Ex}", ex);
            throw;
        }
    }

    private void ConfigurePubSub(String channelId, String twitchApiOAuthToken)
    {
        Guard.FromNullOrWhitespace(channelId, nameof(channelId));

        _twitchPubSub ??= new();
        _twitchPubSub.OnLog += OnPubSubLog;
        _twitchPubSub.OnPubSubServiceConnected += (sender, e) =>
        {
            _logger.LogInformation("[Twitch PubSub] Sending topics to listen to...");
            _twitchPubSub.ListenToChannelPoints(channelId);
            _twitchPubSub.SendTopics(twitchApiOAuthToken);
        };
        _twitchPubSub.OnPubSubServiceError += OnPubSubServiceError;
        _twitchPubSub.OnPubSubServiceClosed += OnPubSubServiceClosed;
        _twitchPubSub.OnListenResponse += OnPubSubListenResponse;
        _twitchPubSub.OnChannelPointsRewardRedeemed += OnRewardRedeemed;

        _logger.LogInformation("[Twitch PubSub] Connecting...");

        _twitchPubSub.Connect();
    }
    #endregion
    #region Event Registrations
    private void OnPubSubLog(object? sender, TwitchLib.PubSub.Events.OnLogArgs e)
    => _logger.LogDebug("[Twitch PubSub] {Data}", e.Data);

    private void OnPubSubServiceError(object? sender, TwitchLib.PubSub.Events.OnPubSubServiceErrorArgs e)
    => _logger.LogError("[Twitch PubSub] ERROR: {Exception}", e.Exception);

    private void OnPubSubServiceClosed(object? sender, EventArgs e)
    => _logger.LogInformation("[Twitch PubSub] Connection Closed");

    private void OnPubSubListenResponse(object? sender, TwitchLib.PubSub.Events.OnListenResponseArgs e)
    {
        if (!e.Successful)
        {
            _logger.LogError("[Twitch PubSub] Failed to listen! Response: {Resposne}", e.Response);
            return;
        }

        _logger.LogInformation("[Twitch PubSub] Listening to {Topic} - {Response}", e.Topic, e.Response);
    }
    #endregion
}

