using Microsoft.AspNetCore.SignalR.Client;
using TheOmenDen.CrowsAgainstHumility.Hubs;

namespace TheOmenDen.CrowsAgainstHumility.Pages;
public partial class CawChat : ComponentBase, IAsyncDisposable
{
    [Inject] NavigationManager NavigationManager { get; init; }

    // flag to indicate chat status
    private bool _isChatting = false;

    // name of the user who will be chatting
    private string _username = String.Empty;

    // on-screen message
    private string _message = String.Empty;

    // new message input
    private string _newMessage = String.Empty;

    // list of messages in chat
    private readonly List<ChatMessage> _messages = new ();

    private string _hubUrl = String.Empty;
    private HubConnection? _hubConnection;

    public async Task Chat()
    {
        // check username is valid
        if (string.IsNullOrWhiteSpace(_username))
        {
            _message = "Please enter a name";
            return;
        }

        try
        {
            // Start chatting and force refresh UI.
            _isChatting = true;
            await Task.Delay(1);

            // remove old messages if any
            _messages.Clear();

            // Create the chat client
            var baseUrl = NavigationManager.BaseUri;

            _hubUrl = baseUrl.TrimEnd('/') + CawHub.HubUrl;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .Build();

            _hubConnection.On<string, string>("Broadcast", BroadcastMessage);

            await _hubConnection.StartAsync();

            await SendAsync($"[Notice] {_username} joined chat room.");
        }
        catch (Exception e)
        {
            _message = $"ERROR: Failed to start chat client: {e.Message}";
            _isChatting = false;
        }
    }

    private void BroadcastMessage(string name, string message)
    {
        var isMine = name.Equals(_username, StringComparison.OrdinalIgnoreCase);

        _messages.Add(new ChatMessage(name, message, isMine, false, isMine ? "sent" : "received"));

        // Inform blazor the UI needs updating
        StateHasChanged();
    }

    private async Task DisconnectAsync()
    {
        if (_isChatting)
        {
            await SendAsync($"[Notice] {_username} left chat room.");

            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();

            _hubConnection = null;
            _isChatting = false;
        }
    }

    private async Task SendAsync(string message)
    {
        if (_isChatting && !string.IsNullOrWhiteSpace(message))
        {
            GetChatSenderStatus(true);

            await _hubConnection.SendAsync("Broadcast", _username, message);

            _newMessage = string.Empty;
        }
    }

    private static String GetChatSenderStatus(Boolean isMine)
    {
        const string chatMessageClass = "chat-message chat-message";

        var attachedIdentifier= isMine ? "--sent" : "--received";

        return $"{chatMessageClass}{attachedIdentifier}";
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}