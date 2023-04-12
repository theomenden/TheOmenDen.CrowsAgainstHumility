using Blazorise;
using Blazorise.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TheOmenDen.CrowsAgainstHumility.Core.Constants;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Models.CrowGames;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Services;
using TheOmenDen.CrowsAgainstHumility.Services.Authentication;
using TheOmenDen.Shared.Enumerations.Structs;
using TheOmenDen.Shared.Extensions;
using TwitchLib.Api.Helix;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class CreateCrowGameComponent : ComponentBase, IDisposable, IAsyncDisposable
{
    private const string ModalTitle = "Player Does Not Exist";
    #region Cascading Parameters
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
    #endregion
    #region Injected Services
    [Inject] private IMessageService MessageService { get; init; }

    [Inject] private IModalService ModalService { get; init; }

    [Inject] private IPackViewService PackViewService { get; init; }

    [Inject] private IPlayerVerificationService PlayerVerificationService { get; init; }

    [Inject] private UserManager<ApplicationUser> UserManager { get; init; }
    #endregion
    #region Private Pack Models
    private IEnumerable<Pack> _packs = Enumerable.Empty<Pack>();
    private List<Guid> _selectedPackIds = new(10);
    private List<string> _selectedPackNames = new(10);
    #endregion
    #region Private User Models
    private User _selectedPlayer;
    private string _playerToAdd = String.Empty;
    private readonly List<User> _players = new(10);
    #endregion
    #region Crow Game Input Models
    private readonly CreateCrowGameInputModel _createModal = new();
    #endregion
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _createModal.RoomCode = GameCodeGenerator.GenerateGameCodeFromComponent(nameof(CreateCrowGameComponent));
        _packs = (await PackViewService.GetPacksForGameCreationAsync()).ToArray();
    }

    #region Pack Manipulation Methods
    private int GetEstimatedWhiteCardCount()
    => _selectedPackIds
            .Sum(selectedPackId =>
                _packs
                    .FirstOrDefault(f => f.Id == selectedPackId)
                    ?.WhiteCardsInPack ?? 0);

    private int GetEstimatedBlackCardCount()
        => _selectedPackIds
            .Sum(selectedPackId =>
                _packs
                    .FirstOrDefault(f => f.Id == selectedPackId)
                    ?.BlackCardsInPack ?? 0);

    private Task AddAllOfficialPacksAsync()
    {
        _selectedPackIds = _packs.Where(f => f.IsOfficialPack)
            .Select(f => f.Id)
            .ToList();
        return Task.CompletedTask;
    }

    private Task AddRandomPackAsync()
    {
        _selectedPackIds.Add(_packs.GetRandomElement().Id);
        return Task.CompletedTask;
    }

    private bool IsOfficialPack(string packName) => _packs.Any(p => p.Name.Equals(packName, StringComparison.OrdinalIgnoreCase) && p.IsOfficialPack);

    private Task Add5RandomPacksAsync()
    {
        var randomPacksToAdd = _packs
            .ExceptBy(_selectedPackIds, p => p.Id)
            .Select(p => p.Id)
            .GetRandomElements(5)
            .ToList();

        _selectedPackIds.AddRange(randomPacksToAdd);

        return Task.CompletedTask;
    }

    private Task ClearPacksAsync()
    {
        _selectedPackNames.Clear();
        _selectedPackIds.Clear();
        return Task.CompletedTask;
    }
    #endregion

    #region  Twitch Methods
    private async Task AddPlayerToListAsync()
    {
        var player = await PlayerVerificationService.CheckTwitchForUserAsync(_playerToAdd);

        if (player is null)
        {
            await MessageService.Warning($"{_playerToAdd} was not able to be verified", ModalTitle, options =>
            {
                options.Size = ModalSize.Small;
                options.TitleClass = "fw-bold";
                options.MessageClass = "fw-light";
                options.ShowMessageIcon = true;
                options.ShowCloseButton = true;
            });
        }

        if (player is not null)
        {
            _players.Add(player);
        }

        _playerToAdd = String.Empty;

        await InvokeAsync(StateHasChanged);
    }

    private static String DeterminePlayerIconName(string broadcasterType)
    {
        if (String.IsNullOrWhiteSpace(broadcasterType))
        {
            return "fa-brands fa-twitch";
        }

        return String.Equals(broadcasterType,BroadcasterTypes.Affiliate.ToString(), StringComparison.InvariantCultureIgnoreCase)
            ? "fa-certificate"
            : "fa-badge-check";
    }


    private static string DeterminePlayerIconColor(string broadcasterType)
    {
        if (String.IsNullOrWhiteSpace(broadcasterType))
        {
            return "#fff";
        }

        return String.Equals(broadcasterType, BroadcasterTypes.Partner.ToString(), StringComparison.InvariantCultureIgnoreCase) 
            ? $"color: #{TwitchColorCodes.MAIN_COLOR};"
            : $"color: #{TwitchColorCodes.ACCENT_COLOR};";
    }

    private Task RemovePlayerAsync(User player)
    {
        _players.Remove(player);
        return Task.CompletedTask;
    }

    #endregion
    private Task OnCancelClickedAsync()
    => ModalService.Hide();
    #region Disposal Methods
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
    #endregion
}
