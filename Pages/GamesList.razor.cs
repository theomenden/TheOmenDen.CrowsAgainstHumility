using Blazorise;
using TheOmenDen.CrowsAgainstHumility.Components;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Services.Authentication;
using TheOmenDen.Shared.Extensions;
using TheOmenDen.Shared.Utilities;

namespace TheOmenDen.CrowsAgainstHumility.Pages;

public partial class GamesList : ComponentBase, IDisposable
{
    [Inject] public NavigationManager Navigation { get; init; }

    [Inject] private IModalService ModalService { get; init; }

    [Inject] private ICrowGameHubConnectorService CrowGameHubConnectorService { get; init; }

    private List<CrowGame> _games = new (52);

    private void Start()
    {
        Navigation.NavigateTo("/play");
    }

    protected override async Task OnInitializedAsync()
    {
        _games =Enumerable.Empty<CrowGame>().ToList();

        await base.OnInitializedAsync();
    }

    private Task InstantiateModalAsync()
        => ModalService.Show<CreateCrowGameComponent>(String.Empty, new ModalInstanceOptions()
        {
            Scrollable=true,
            Size= ModalSize.ExtraLarge,
            Border= Border.Is2.Light.OnAll.Rounded,
            Centered = true,
            UseModalStructure=false
        });
    

    private void OnGameListChanged(object? sender, EventArgs e) => StateHasChanged();

    private void OnGameSettingsChanged(object? sender, EventArgs e) => StateHasChanged();

    private Task JoinRoomAsync(String roomName, String roomCode)
    {
        return InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}