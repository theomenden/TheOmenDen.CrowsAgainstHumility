using Blazorise;
using Blazorise.Components;
using Microsoft.EntityFrameworkCore;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Services;
using TheOmenDen.CrowsAgainstHumility.Services.Authentication;
using TheOmenDen.Shared.Extensions;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class CreateCrowGameComponent : ComponentBase
{
    private const string ModalTitle = "Player Does Not Exist";
    
    [Inject] private IDbContextFactory<CrowsAgainstHumilityContext> DbContextFactory { get; init; }

    [Inject] private ICardPoolBuildingService CardPoolService { get; init; }

    [Inject] private IPlayerVerificationService PlayerVerificationService { get; init; }

    [Inject] private IMessageService MessageService { get; init; }

    [Inject] private IModalService ModalService { get; init; }

    [Inject] private CrowGameService StateManager { get; init; }

    private Steps _stepsRef;
    private string _selectedStep = "1";
    private Int32 _totalCardsInPool = 0;

    private string _playerName = String.Empty;

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

    private string GenerateGameCode()
    {
        _gameCode = GameCodeGenerator.GenerateGameCode();

        if (!StringBuilderPoolFactory<GameCodeGenerator>.Exists(nameof(GameCodeGenerator)))
        {
            return _gameCode;
        }

        var sb = StringBuilderPoolFactory<GameCodeGenerator>.Get(nameof(GameCodeGenerator));

        sb!.Clear();

        return GameCodeGenerator.GenerateGameCode();
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

    private Task OnCancelClicked()
    => ModalService.Hide();
}
