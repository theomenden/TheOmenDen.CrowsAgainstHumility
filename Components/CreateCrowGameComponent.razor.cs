using Blazorise;
using Blazorise.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
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

    [Inject] private IDbContextFactory<CrowsAgainstHumilityContext> DbContextFactory { get; init; }

    [Inject] private ICardPoolBuildingService CardPoolService { get; init; }

    [Inject] private IPlayerVerificationService PlayerVerificationService { get; init; }

    [Inject] private ICrowGameService CrowGameService { get; init; }

    [Inject] private IMessageService MessageService { get; init; }

    [Inject] private IModalService ModalService { get; init; }

    [Inject] private CrowGameService StateManager { get; init; }

    [Inject] private UserManager<ApplicationUser> UserManager { get; init; }

    private Steps _stepsRef;
    private string _selectedStep = "1";
    private Int32 _totalCardsInPool = 0;

    private User _selectedPlayer;

    private string _playerToAdd = String.Empty;

    private readonly List<User> _players = new(10);

    private List<Pack> _packs = new(300);

    private readonly Dictionary<Guid, Pack> _packsToChoose = new(10);

    private List<Pack> _chosenPacks = new(10);

    private List<string> _chosenPackTexts = new(10);

    private bool _isIndicatorVisible;

    private Pack _selectedPack;

    private string _gameName;

    private string _gameCode = String.Empty;

    private bool _enabledCustomFilters = false;
    protected override void OnInitialized()
    {
        _gameCode = GameCodeGenerator.GenerateGameCodeFromComponent(nameof(CreateCrowGameComponent)); ;
    }

    private void InitializeNewGame()
    {
        StateManager.Game = new CrowGame()
        {
            Name = _gameName
        };
    }

    private async Task OnHandleReadData(AutocompleteReadDataEventArgs autocompleteReadDataEventArgs)
    {
        if (!autocompleteReadDataEventArgs.CancellationToken.IsCancellationRequested)
        {
            _packs = await CardPoolService
                .GetPacksBySearchValueAsync(autocompleteReadDataEventArgs.SearchValue, autocompleteReadDataEventArgs.CancellationToken)
                .ToListAsync();
        }
    }

    private async Task AddPlayerToList()
    {
        var player = await PlayerVerificationService.CheckTwitchForUser(_playerToAdd);

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
    }

    private static String DetermineIconName(String username) =>
        String.IsNullOrWhiteSpace(username)
            ? "bi bi-x-circle-fill"
            : "bi bi-patch-check-fill";

    private static TextColor DetermineIconColor(String username) =>
        String.IsNullOrWhiteSpace(username)
            ? TextColor.Danger
            : TextColor.Success;

    private async Task AddOfficialPacks()
    {
        _isIndicatorVisible = true;

        var packs = await CardPoolService.GetOfficialPacksAsync();

        foreach (var pack in packs)
        {
            if (_packsToChoose.TryAdd(pack.Id, pack))
            {
                _totalCardsInPool += pack.WhiteCardsInPack;
            }
            await InvokeAsync(StateHasChanged);
        }

        _isIndicatorVisible = false;
    }

    private async Task AddSearchedPacks()
    {
        _isIndicatorVisible = true;

        foreach (var pack in _chosenPacks)
        {
            if (_packsToChoose.TryAdd(pack.Id, pack))
            {
                _totalCardsInPool += pack.WhiteCardsInPack;
            }
            await InvokeAsync(StateHasChanged);
        }

        _isIndicatorVisible = false;
    }

    private async Task AddRandomPacks()
    {
        _isIndicatorVisible = true;

        foreach (var pack in await CardPoolService.GetRandomPacksAsync()
                           .ToArrayAsync())
        {
            if (_packsToChoose.TryAdd(pack.Id, pack))
            {
                _totalCardsInPool += pack.WhiteCardsInPack;
            }

            await InvokeAsync(StateHasChanged);
        }

        _isIndicatorVisible = false;
    }

    private Task RemovePack()
    {
        _isIndicatorVisible = true;

        if (_selectedPack is null)
        {
            _isIndicatorVisible = false;
            return Task.CompletedTask;
        }

        _packsToChoose.Remove(_selectedPack.Id);
        _chosenPacks.Remove(_selectedPack);
        _chosenPackTexts.Remove(_selectedPack.Name);
        _totalCardsInPool -= _selectedPack.WhiteCardsInPack;

        _isIndicatorVisible = false;

        return Task.CompletedTask;
    }

    private Task RemoveAllPacks()
    {
        _isIndicatorVisible = true;
        _packsToChoose.Clear();
        _chosenPacks.Clear();
        _chosenPackTexts.Clear();
        _totalCardsInPool = 0;
        _isIndicatorVisible = false;

        StateHasChanged();

        return Task.CompletedTask;
    }

    private Task RemovePlayer(String playerId)
    {
        _selectedPlayer = _players.FirstOrDefault(p => p.Id == playerId);

        if (_selectedPlayer is not null && _players.Any(p => p.Email == _selectedPlayer.Email))
        {
            _players.Remove(_selectedPlayer);
        }

        return Task.CompletedTask;
    }

    private bool IsNavigationAllowed(StepNavigationContext context)
    {
        if (context.CurrentStepIndex is 4
            && context.NextStepIndex is 5
            && !_packsToChoose.Any()
            && _players.Count < 10)
        {
            return false;
        }

        return true;
    }

    private async Task CreateCrowGameAsync()
    {
        var authState = await AuthenticationStateTask;

        var currentUser = await UserManager.GetUserAsync(authState.User);

        var playerNames = _players.Select(p => p.DisplayName).ToArray();

        var newCrowGame = new CrowGameCreator(_packs, currentUser.Id, playerNames , _gameName, _gameCode);

        await CrowGameService.CreateCrowGameAsync(newCrowGame);
    }

    private Task OnResetClicked()
    {
        _players.Clear();
        _packsToChoose.Clear();
        _gameName = String.Empty;
        _gameCode = String.Empty;
        _totalCardsInPool = 0;
        return Task.CompletedTask;
    }

    private Task OnCancelClicked()
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
