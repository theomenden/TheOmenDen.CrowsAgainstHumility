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
    }
    

    public async ValueTask DisposeAsync()
    {
        await HubConnection.DisposeAsync();
        GC.SuppressFinalize(this);
        await ValueTask.CompletedTask;
    }
}