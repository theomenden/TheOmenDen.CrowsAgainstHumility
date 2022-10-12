using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Delegates;
using TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
using TheOmenDen.Shared.Guards;

namespace TheOmenDen.CrowsAgainstHumility.Services.Processing;
internal sealed class GameClient: IAsyncDisposable
{
    public const string HubUrl = "/CrowGame";

    private readonly string _hubUrl;

    private readonly string _username;

    private bool _isStarted;

    private HubConnection _hubConnection;

    private readonly ILogger<GameClient> _logger;

    public event MessageReceivedEventHandler MessageReceived;

    public GameClient(ILogger<GameClient> logger,string username, string baseUri)
    {
        Guard.FromNullOrWhitespace(username, nameof(username));
        Guard.FromNullOrWhitespace(baseUri, nameof(baseUri));

        _logger = logger;

        _username = username;
        _hubUrl = $"{baseUri.Trim('/')}{HubUrl}";
    }

    public async Task StartClientAsync()
    {
        if (!_isStarted)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .Build();

            _logger.LogInformation("Calling {start}()", nameof(StartClientAsync));

            _hubConnection.On<string, string>("", (user, message) =>
            {
                HandleReceivedMessage(user, message);
            });

            await _hubConnection.StartAsync();

            _logger.LogInformation("Started Client");

            _isStarted = true;

            await _hubConnection.SendAsync("", _username);
        }
    }

    public async Task SendAsync(string message)
    {
        if (!_isStarted)
        {
            throw new InvalidOperationException("Client not started");
        }

        await _hubConnection.SendAsync("", _username, message);
    }

    public async ValueTask StopAsync()
    {
        if (_isStarted)
        {
            await _hubConnection.StopAsync();

            await _hubConnection.DisposeAsync();
        }

        _hubConnection = null;
        _isStarted = false;
    }

    private void HandleReceivedMessage(string username, string message)
    {
        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(username, message));
    }

    public ValueTask DisposeAsync()
    {
        _logger.LogWarning("Disposing Game");
        return StopAsync();
    }
}

