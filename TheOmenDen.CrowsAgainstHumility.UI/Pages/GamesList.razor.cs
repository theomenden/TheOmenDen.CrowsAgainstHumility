using Blazorise;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Clients;
using TheOmenDen.CrowsAgainstHumility.Components;
using TheOmenDen.CrowsAgainstHumility.Components.Game;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Stores;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.Shared.Extensions;
using TheOmenDen.Shared.Utilities;

namespace TheOmenDen.CrowsAgainstHumility.Pages;

public partial class GamesList : ComponentBase, IDisposable
{
    #region Parameters
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
    [Parameter] public ICrowGameHubClient CrowGameHubClient { get; set; }
    #endregion
    #region Injected Members
    [Inject] public NavigationManager Navigation { get; init; }
    [Inject] private IUserService UserService { get; set; }
    [Inject] private IModalService ModalService { get; init; }
    [Inject] private IServerStore _serverStore { get; init; }
    #endregion
    #region Fields
    private AuthenticationState _authState;
    private List<CrowGameServer> _games = new(52);
    private bool _showContextMenu  = false;
    #endregion
    protected override async Task OnInitializedAsync()
    {
        _games = await _serverStore.GetAllServersAsyncStream().ToListAsync();

        _authState = await AuthenticationStateTask;

        await base.OnInitializedAsync();
    }

    private Task InstantiateModalAsync()
        => ModalService.Show<CreateCrowGameComponent>(String.Empty, new ModalInstanceOptions()
        {
            Scrollable = true,
            Size = ModalSize.ExtraLarge,
            Border = Border.Is2.Light.OnAll.Rounded,
            Centered = true,
            UseModalStructure = false
        });

    private Task InstantiateJoinModalAsync(Guid serverId) =>
        ModalService.Show<JoinCrowGameComponent>(String.Empty, new ModalInstanceOptions
        {
            Scrollable = true,
            Size = ModalSize.Large,
            Border = Border.Is2.Light.OnAll.Rounded,
            Centered = true,
            UseModalStructure = false
        });

    protected async Task OnContextItemEditClicked(CrowGameServer server)
    {
        await InstantiateJoinModalAsync(server.Id);
        _showContextMenu = false;
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}