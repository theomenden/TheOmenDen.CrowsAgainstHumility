using Blazorise;
using Blazorise.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Services;
using TheOmenDen.CrowsAgainstHumility.Services.Authentication;
using TheOmenDen.Shared.Extensions;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class CreateCrowGameComponent : ComponentBase, IDisposable, IAsyncDisposable
{
    private const string ModalTitle = "Player Does Not Exist";

    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

    [Inject] private IModalService ModalService { get; init; }

    [Inject] private IPackViewService PackViewService { get; init; }

    private IEnumerable<Pack> _packs = Enumerable.Empty<Pack>();

    private readonly CreateCrowGameInputModel _createModal = new ();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _createModal.RoomCode = GameCodeGenerator.GenerateGameCodeFromComponent(nameof(CreateCrowGameComponent));
        _packs = await PackViewService.GetPacksForGameCreationAsync();
    }

    private Task OnResetClickedAsync()
    {
        return Task.CompletedTask;
    }

    private Task OnCancelClickedAsync()
    => ModalService.Hide();

    public void Dispose()
    {
        StringBuilderPoolFactory<GameCodeGenerator>.Remove(nameof(CreateCrowGameComponent));
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        StringBuilderPoolFactory<GameCodeGenerator>.Remove(nameof(CreateCrowGameComponent));
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
