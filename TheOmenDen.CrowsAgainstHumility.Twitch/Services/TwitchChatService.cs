using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.Shared.Guards;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using TwitchLib.PubSub.Events;

namespace TheOmenDen.CrowsAgainstHumility.Twitch.Services;

internal sealed class TwitchChatService: IDisposable, IAsyncDisposable, ITwitchChatService
{
    private readonly ILogger<TwitchChatService> _logger;

    private readonly IMessageChannelService<TwitchChatMessage> _messageChannelService;

    private ITwitchClient? _twitchClient;

    private String _channelToInteractWith = String.Empty;

    private bool _disposedValue;

    #region Event Handlers
    public event EventHandler<OnMessageReceivedArgs>? OnMessageReceived;
    public event EventHandler<OnChannelPointsRewardRedeemedArgs>? OnRewardRedeemed;
    public event EventHandler<OnJoinedChannelArgs>? OnConnected;
    public event EventHandler<OnDisconnectedEventArgs>? OnDisconnected;
    #endregion

    public String Username { get; private set; } = String.Empty;
    
    public TwitchChatService(ILogger<TwitchChatService> logger)
    {
        _logger = logger;
    }

    public Task ConnectToChannelAsync(String channel, String oauthToken, String username, String clientId,
        CancellationToken cancellationToken = new())
    {
        _logger.LogDebug("ConnectToChannelAsync({Channel}, {OAuthToken}, {Username}, {ClientId})",
            channel, oauthToken, username, clientId);

        Guard.FromNullOrWhitespace(channel, nameof(channel));
        Guard.FromNullOrWhitespace(oauthToken, nameof(oauthToken));
        Guard.FromNullOrWhitespace(username, nameof(username));

        ConfigureClient(username, oauthToken, channel);
        _channelToInteractWith = channel;
        Username = username;
        return Task.CompletedTask;
    }

    public void SendMessage(String message)
    {
        if (!IsConnectedToTwitch())
        {
            _logger.LogWarning("[Twitch Client] is not connected");
            return;
        }

        _twitchClient?.SendMessage(_channelToInteractWith, message);
    }
    public async Task SendMessage(string message, String sender = "mg36crow", CancellationToken cancellationToken = default)
    {
        var content = new TwitchChatMessage(message, sender, Username);
       
        await _messageChannelService.PublishMessageAsync(content, cancellationToken);;
    }
    public Boolean IsConnectedToTwitch() => _twitchClient is not null && _twitchClient.IsConnected;
    
    private void ConfigureClient(String username, String twitchOAuth, String channel)
    {
        _logger.LogDebug("[Twitch Client] Creating...");

        var credentials = new ConnectionCredentials(username, twitchOAuth);

        _twitchClient ??= new TwitchClient();

        _twitchClient.Initialize(credentials, channel);

        _twitchClient.OnLog += OnClientLog;
        _twitchClient.OnJoinedChannel += OnConnected;
        _twitchClient.OnMessageReceived += OnMessageReceived;
        _twitchClient.OnConnected += OnConnectedClient;
        _twitchClient.OnDisconnected += OnDisconnected;

        _logger.LogDebug("[Twitch Client] Connecting...");

        _twitchClient.Connect();
    }
    #region Resource Cleanup
    public void DisconnectFromTwitch()
    {
        _logger.LogDebug("Disconnecting from Twitch");
     
        if (_twitchClient is null)
        {
            return;
        }

        _twitchClient.Disconnect();
        _twitchClient = null;
    }

    public ValueTask DisconnectFromTwitchAsync()
    {
        _logger.LogDebug("Disconnecting from Twitch");

        if (_twitchClient is null)
        {
            return ValueTask.CompletedTask;
        }

        _twitchClient.Disconnect();
        _twitchClient = null;
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

    public ValueTask DisposeAsync() => DisconnectFromTwitchAsync();
    
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
    #region Private Methods
    private void OnClientLog(object? sender, TwitchLib.Client.Events.OnLogArgs e)
    {
        _logger.LogDebug("[TwitchClient] {LoggedOn}: {Username} - {Data}", e.DateTime, e.BotUsername, e.Data);
    }

    private void OnConnectedClient(object? sender, OnConnectedArgs e)
    {
        _logger.LogInformation("[Twitch Client] Connected to Twitch with username {Username}", e.BotUsername);
    }
    #endregion
}
