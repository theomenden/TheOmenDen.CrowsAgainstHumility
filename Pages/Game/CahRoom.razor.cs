using Blazorise;
using Microsoft.AspNetCore.Components.Web;
using TheOmenDen.CrowsAgainstHumility.Components;
using TheOmenDen.CrowsAgainstHumility.ViewModels;

namespace TheOmenDen.CrowsAgainstHumility.Pages.Game;

public partial class CahRoom : GameModelComponentBase<RoomViewModel>
{
    #region Parameters
    [Parameter]
    public string? RoomCode { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name= "Name")]
    public string? Name { get; set; }
    #endregion
    #region Injected Services
    [Inject]
    private NavigationManager NavigationManger { get; init; }

    [Inject]
    private IModalService ModalService { get; init; }
    #endregion
    #region Private Members
    private IEnumerable<Player> _players = Enumerable.Empty<Player>();
    #endregion
    #region Lifecycle Methods
    protected override async Task OnInitializedAsync()
    {
        ViewModel.RoomCode = RoomCode;
        _players = ViewModel.RoomStateDto?.Players.ToList() ?? Enumerable.Empty<Player>();
        await base.OnInitializedAsync();
    }
    #endregion
    #region Private Methods

    private static Icon DisplayPlayedCardIcon(WhiteCard? playedCard)
        => new ()
        {
            Name = playedCard is not null
                ? IconName.CheckCircle
                : IconName.TimesCircle,
            TextColor = playedCard is not null
                ? TextColor.Success
                : TextColor.Danger,
            IconStyle = IconStyle.DuoTone
        };

    private async Task OnKeyPress(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await ViewModel.PlayCardDialogAsync();
        }
    }
    #endregion
}
