using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Twitch.Interfaces;
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
    private TwitchClient? _twitchClient;
    private TwitchAPI? _twitchAPI;
    private TwitchPubSub? _twitchPubSub;

    private String _channelToInteractWith = String.Empty;
    private Boolean disposedValue;
    #endregion
    public TwitchRedemptionsManagerService(ILogger<TwitchRedemptionsManagerService> logger) => _logger = logger;
    public String Username { get; private set; } = String.Empty;
    #region Event Handlers
    public event EventHandler<OnMessageReceivedArgs>? OnMessageReveived;
    public event EventHandler<OnChannelPointsRewardRedeemedArgs>? OnRewardRedeemed;
    public event EventHandler<OnJoinedChannelArgs>? OnConnected;
    public event EventHandler<OnDisconnectedEventArgs>? OnDisconnected;
    #endregion
    #region Connection Methods
    public async Task ConnectToChannelAsync(String channel, String oauthToken, String username, String clientId, CancellationToken cancellationToken = new())
    {
        InitializeChannelConnections(channel, oauthToken, username, clientId);

        var channelId = await ConfigureApi(oauthToken, clientId, channel, cancellationToken);
        
        ConfigureClient(username,oauthToken, channel);
        
        ConfigurePubSub(channelId, oauthToken);
    }
    private void InitializeChannelConnections(String channel, String oauthToken, String username, String clientId)
    {
        _logger.LogDebug("InitializeChannelConnection({Channel}, {OAuthToken}, {Username}, {ClientId})",
            channel, oauthToken, username, clientId);

        Guard.FromNullOrWhitespace(channel, nameof(channel));
        Guard.FromNullOrWhitespace(oauthToken, nameof(oauthToken));
        Guard.FromNullOrWhitespace(username, nameof(username));

        _channelToInteractWith = channel;
        Username = username;

        _twitchAPI    ??= new();
        _twitchClient ??= new();
        _twitchPubSub ??= new();
    }
    #endregion
    public Boolean IsConnectedToTwitch() => _twitchClient is not null && _twitchClient.IsConnected;

    public void SendMessage(String message)
    {
        if (!IsConnectedToTwitch())
        {
            _logger.LogWarning("[Twitch Client] is not connected");
            return;
        }

        _twitchClient?.SendMessage(_channelToInteractWith, message);
    }

    #region Resource Cleanup
    public void DisconnectFromTwitch()
    {

        _logger.LogDebug("Disconnecting from Twitch");

        if (_twitchClient is not null)
        {
            _twitchClient.Disconnect();
            _twitchClient = null;
        }

        if (_twitchPubSub is not null)
        {
            _twitchPubSub.Disconnect();
            _twitchPubSub = null;
        }

        if (_twitchAPI is not null)
        {
            _twitchAPI = null;
        }
    }

    public ValueTask DisconnectFromTwitchAsync()
    {
        _logger.LogDebug("Disconnecting from Twitch");

        if (_twitchClient is not null)
        {
            _twitchClient.Disconnect();
            _twitchClient = null;
        }

        if (_twitchPubSub is not null)
        {
            _twitchPubSub.Disconnect();
            _twitchPubSub = null;
        }

        if (_twitchAPI is not null)
        {
            _twitchAPI = null;
        }

        return ValueTask.CompletedTask;
    }

    private void Dispose(Boolean disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                DisconnectFromTwitch();
            }

            disposedValue = true;
        }
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
        _twitchAPI ??= new();

        var twitchApiOauthToken = String.Empty;

        if (oauthToken.StartsWith("oath:"))
        {
            twitchApiOauthToken = oauthToken.Substring("oauth:".Length);
        }

        _twitchAPI.Settings.AccessToken = twitchApiOauthToken;
        _twitchAPI.Settings.ClientId = clientId;

        String channelId;

        try
        {
            _logger.LogDebug("[Twitch API] Trying to find channel ID...");

            var response = await _twitchAPI.Helix.Users.GetUsersAsync(null, new() { channel });

            if (response.Users.Length is 1)
            {
                channelId = response.Users[0].Id;
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

    private void ConfigureClient(String username, String twitchOAuth, String channel)
    {
        _logger.LogDebug("[Twitch Client] Creating...");

        var credentials = new ConnectionCredentials(username, twitchOAuth);

        _twitchClient ??= new();

        _twitchClient.Initialize(credentials, channel);

        _twitchClient.OnLog += OnClientLog;
        _twitchClient.OnJoinedChannel += OnConnected;
        _twitchClient.OnMessageReceived += OnMessageReveived;
        _twitchClient.OnConnected += OnConnectedClient;
        _twitchClient.OnDisconnected += OnDisconnected;

        _logger.LogDebug("[Twitch Client] Connecting...");

        _twitchClient.Connect();
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
    private void OnConnectedClient(object? sender, OnConnectedArgs e)
    {
        _logger.LogInformation("[Twitch Client] Connected to Twitch with username {Username}", e.BotUsername);
    }

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

    private void OnClientLog(object? sender, TwitchLib.Client.Events.OnLogArgs e)
    {
        _logger.LogDebug("[TwitchClient] {LoggedOn}: {Username} - {Data}", e.DateTime, e.BotUsername, e.Data);
    }
    #endregion
}

