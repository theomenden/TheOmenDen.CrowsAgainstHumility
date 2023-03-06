using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Delegates;
using TheOmenDen.CrowsAgainstHumility.Core.Messages;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
using TheOmenDen.Shared.Guards;

namespace TheOmenDen.CrowsAgainstHumility.Services.Clients;
public sealed class ChatClient: IAsyncDisposable
{
    public const string HubUrl = "/chatHub";
    public event MessageReceivedEventHandler OnMessageReceived;
    private readonly string _hubUrl;
    private readonly ApplicationUser _user;
    private bool _isStarted;

    private HubConnection _hubConnection;

    public ChatClient(ApplicationUser user, string siteUrl)
    {
        Guard.FromNull(user, nameof(user));
        Guard.FromNullOrWhitespace(siteUrl, nameof(siteUrl));

        _user = user;
        _hubUrl = siteUrl.TrimEnd('/') + HubUrl;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (!_isStarted)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .Build();

            _hubConnection.On<string, string>(Messages.Received, HandleReceiveMessage);

            await _hubConnection.StartAsync(cancellationToken);

            _isStarted = true;

            await _hubConnection.SendAsync(Messages.Registered,_user.UserName,cancellationToken);
        }
    }

    public async Task SendAsync(CawChatMessageDto message, CancellationToken cancellationToken = default)
    {
        if (!_isStarted)
        {
            throw new InvalidOperationException("Chat client not started");
        }

        await _hubConnection.SendAsync(Messages.Sent, message.FromUser.UserName, message.Message, cancellationToken);
    }

    private void HandleReceiveMessage(String username, String message) =>
        OnMessageReceived?.Invoke(this, new MessageReceivedEventArgs(username, message));
    
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!_isStarted)
        {
            return;
        }

        await _hubConnection.StopAsync(cancellationToken);

        await _hubConnection.DisposeAsync();

        _hubConnection = null;
        _isStarted = false;
    }

    public async ValueTask DisposeAsync() => await StopAsync();
    
}
