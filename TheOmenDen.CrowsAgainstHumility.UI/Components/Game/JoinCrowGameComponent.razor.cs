using Blazorise.LoadingIndicator;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using TheOmenDen.CrowsAgainstHumility.Azure.Clients;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Storage;

namespace TheOmenDen.CrowsAgainstHumility.Components.Game;

public partial class JoinCrowGameComponent : ComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
    [Inject] private IUserService UserService { get; init; }
    [Inject] private IServerStore CrowGameStore { get; init; }
    [Inject] private IServerSessionManagementMechanism ServerSessionManagementMechanism { get; set; }
    [Inject] private NavigationManager NavigationManager { get; init; }
    #region Fields

    private LoadingIndicator _loadingIndicator;
    private AuthenticationState? _authState;
    private ICrowGameHubClient _hubClient;
    private string _suppliedLobbyCode = String.Empty;
    private string _username = String.Empty;
    private int _selectJoiningRole = GameRoles.Observer.Value;
    #endregion

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var connection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/hubs/crowGame"))
            .WithAutomaticReconnect()
            .Build();

        _hubClient = new CrowGameHubClient(connection);


        _authState ??= await AuthenticationStateTask;

        _username = _authState.User.Identity?.Name ?? await GetUsernameAsync();
    }

    private async ValueTask<String> GetUsernameAsync()
    {
        var userId = _authState!.User.GetUserId();

        var user = await UserService.GetUserViewModelAsync(userId);

        return user.Username ?? user.Email;
    }

    private async Task JoinLobbyAsync()
    {
        await _loadingIndicator.Show();
        var lobbyToJoin = await CrowGameStore.GetServerByCodeAsync(_suppliedLobbyCode);

        var roleToJoinAs = GameRoles.ParseFromValue(_selectJoiningRole);

        var player = await _hubClient.JoinServer(lobbyToJoin.Id, _authState!.User.GetUserId(), _username, roleToJoinAs);

        var serverSession = new ServerSession(lobbyToJoin.Id, player.Username, player.RecoveryId!.Value, roleToJoinAs);

        await ServerSessionManagementMechanism.SetSessionAsync(serverSession);

        await InvokeAsync(StateHasChanged);

        await _loadingIndicator.Hide();
    }
}
