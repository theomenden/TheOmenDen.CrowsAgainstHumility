using Microsoft.AspNetCore.Components.Authorization;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
using TheOmenDen.CrowsAgainstHumility.Services.Clients;

namespace TheOmenDen.CrowsAgainstHumility.Pages;
public partial class CawChat : ComponentBase, IAsyncDisposable
{
    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; }
    [Inject] private ILogger<CawChat> Logger { get; init; }
    [Inject] private IUserService UserService { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; }
    private bool _isChatting = false;
    private bool _isDisposed = false;
    private ApplicationUser _fromUser;
    private string _notLoggedInMessage = String.Empty;
    private string _message = String.Empty;
    private string _newMessage = String.Empty;
    private ChatClient? _chatClient;
    private List<CawChatMessageDto> _messages = new(20);

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateTask.ConfigureAwait(false);

        var currentUser = await UserService.GetUserByUsernameAsync(authState.User.Identity?.Name ?? String.Empty);

        if (currentUser is not null)
        {
            _fromUser = currentUser;

            await ChatAsync();
        }
    }

    private async Task ChatAsync()
    {
        if (_fromUser is null)
        {
            _notLoggedInMessage = "Please login to chat";
            return;
        }

        try
        {
            _messages.Clear();

            var baseUrl = NavigationManager.BaseUri;

            _chatClient = new ChatClient(_fromUser, baseUrl);

            _chatClient.OnMessageReceived += MessageReceived;

            Logger.LogInformation("Attempting to start chat");
            await _chatClient.StartAsync();
            Logger.LogInformation("Chat started");

            _isChatting = true;
        }
        catch (Exception e)
        {
            _message = $"ERROR: Failed to start chat client: {e.Message}";

            Logger.LogError("Exception in chat client: {@Ex}", e);
        }
    }

    private void MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Logger.LogInformation("Blazor: received: {Username}: {Message}", e.Username, e.Message);

        var isMine = false;

        if (String.IsNullOrWhiteSpace(e.Username))
        {
            isMine = String.Equals(e.Username, _fromUser.UserName, StringComparison.OrdinalIgnoreCase);
        }

        var nextMessage = new CawChatMessageDto(Guid.NewGuid(), DateTime.UtcNow, _fromUser, _fromUser, _message);

        _messages.Add(nextMessage);

        StateHasChanged();
    }

    private async Task DisconnectAsync()
    {
        if (_isChatting)
        {
            await _chatClient!.StopAsync();
            _chatClient = null;
            _message = "Chat closed";
            _isChatting = false;
        }
    }

    private async Task SendAsync()
    {
        if (_isChatting && !String.IsNullOrWhiteSpace(_newMessage))
        {
            var messageToSend = new CawChatMessageDto(Guid.NewGuid(),DateTime.UtcNow, _fromUser ,_fromUser, _newMessage);

            await _chatClient!.SendAsync(messageToSend);

            _newMessage = String.Empty;
        }

        await InvokeAsync(StateHasChanged);
    }

    public ValueTask DisposeAsync()
    {
        if (_isDisposed || _chatClient is null)
        {
            return ValueTask.CompletedTask;
        }

        _isDisposed = true;
        return _chatClient.DisposeAsync();
    }
}