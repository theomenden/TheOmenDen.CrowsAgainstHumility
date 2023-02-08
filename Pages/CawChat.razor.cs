using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using TheOmenDen.CrowsAgainstHumility.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Pages;
public partial class CawChat : ComponentBase, IAsyncDisposable
{
    #region Parameters
    [CascadingParameter] public HubConnection HubConnection { get; set; }
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
    [Parameter] public String CurrentMessage { get; set; }
    [Parameter] public Guid CurrentUserId { get; set; }
    [Parameter] public Guid ContactId { get; set; }
    [Parameter] public String ContactEmail { get; set; }
    [Parameter] public String CurrentUserEmail { get; set; }
    #endregion
    #region Injected Members
    [Inject] private NavigationManager NavigationManager { get; init; }
    #endregion
    private List<CawChatMessage> _messages = new (20);
    private List<ApplicationUser> _chatUsers = Enumerable.Empty<ApplicationUser>().ToList(); 
    protected override async Task OnInitializedAsync()
    {
        HubConnection ??= new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/chat"))
            .Build();

        if (HubConnection.State is HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }

        var state = await AuthenticationStateTask;

        var user = state.User;

        CurrentUserId = user.GetUserId();
        CurrentUserEmail = user.Claims.Where(a => a.Type == "name").Select(a => a.Value).FirstOrDefault() ?? String.Empty;
        if (CurrentUserId != Guid.Empty)
        {
            await LoadUserChatMessagesAsync(CurrentUserId);
        }
    }

    private async Task LoadUserChatMessagesAsync(Guid userId)
    {
        var contact = await _cawChatManager.GetUserDetailsAsync(userId);
        ContactId = contact.Id;
        ContactEmail = contact.Email;

        NavigationManager.NavigateTo("chat/ContactId");

        _messages = await _cawChatManager.GetConversationAsync(ContactId);
    }

    private async Task GetUsersAsync()
    {
        _chatUsers = await _cawChatManager.GetAllUsersAsync();
    }

    private async Task RegisterHubOperationsAsync()
    {
        HubConnection.On<CawChatMessage, String>("ReceiveMessage", async (message, userName) =>
        {
            if ((ContactId == message.ToUserId && CurrentUserId == message.FromUserId)
                || (ContactId == message.FromUserId && CurrentUserId == message.ToUserId))
            {
                if (ContactId == message.ToUserId && CurrentUserId == message.FromUserId)
                {
                    _messages.Add(new CawChatMessage
                    {
                        Message = message.Message,
                        CreatedAt = message.CreatedAt,
                        FromUser = new() { Email = CurrentUserEmail }
                    });

                    await HubConnection.SendAsync("ChatNotificationAsync", $"New Message From {userName}", ContactId,
                        CurrentUserId);
                }

                if (ContactId == message.FromUserId && CurrentUserId == message.ToUserId)
                {
                    _messages.Add(new CawChatMessage
                    {
                        Message = message.Message,
                        CreatedAt = message.CreatedAt,
                        FromUser = new() { Email = ContactEmail }
                    });
                }

                StateHasChanged();
            }
        });
    }

    public async ValueTask DisposeAsync()
    {
        await HubConnection.DisposeAsync();
        GC.SuppressFinalize(this);
        await ValueTask.CompletedTask;
    }
}