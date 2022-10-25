using Blazorise;
using TheOmenDen.CrowsAgainstHumility.Components;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;

namespace TheOmenDen.CrowsAgainstHumility.Pages;

public partial class GamesList : ComponentBase
{
    [Inject] public NavigationManager Navigation { get; init; }

    [Inject] private IModalService ModalService { get; init; }

    [Inject] private ICrowGameService CrowGameService { get; init; }

    private IEnumerable<CrowGameDto> _games = Enumerable.Empty<CrowGameDto>();

    private void Start()
    {
        Navigation.NavigateTo("/play");
    }

    protected override async Task OnInitializedAsync()
    {
        _games = Enumerable.Empty<CrowGameDto>();

        await base.OnInitializedAsync();
    }


    private Task InstantiateModalAsync()
        => ModalService.Show<CreateCrowGameComponent>(String.Empty, new ModalInstanceOptions()
        {
            Scrollable=true,
            Size= ModalSize.ExtraLarge,
            UseModalStructure=false
        });
}