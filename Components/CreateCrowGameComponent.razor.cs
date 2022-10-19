using Blazorise;
using Blazorise.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Specialized;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Services.Interfaces;
using TheOmenDen.Shared.Utilities;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class CreateCrowGameComponent : ComponentBase
{
    private const string ModalTitle = "Player Does Not Exist";

    [Parameter] public Boolean IsVisible { get; init; }

    [Inject] public IDbContextFactory<CrowsAgainstHumilityContext> DbContextFactory { get; init; }

    [Inject] public ICardPoolBuildingService CardPoolService { get; init; }

    [Inject] public IPlayerVerificationService PlayerVerificationService { get; init; }

    [Inject] public IMessageService MessageService { get; init; }

    private Int32 _totalCardsInPool = 0;

    private string _playerName = String.Empty;

    private string _playerToAdd = String.Empty;

    private readonly List<User> _players = new(10);

    private List<Pack> _packs = new (300);

    private readonly Dictionary<Guid, Pack> _packsToChoose = new(10);

    private List<Pack> _chosenPacks = new(10);

    private List<string> _chosenPackTexts = new(10);

    private bool _isIndicatorVisible;

    private Pack _selectedPack;

    private async Task OnHandleReadData(AutocompleteReadDataEventArgs autocompleteReadDataEventArgs)
    {
        if (!autocompleteReadDataEventArgs.CancellationToken.IsCancellationRequested)
        {
            _packs = await CardPoolService.GetPacksBySearchValueAsync(autocompleteReadDataEventArgs.SearchValue, autocompleteReadDataEventArgs.CancellationToken)
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

    private Task OnPlayersUpdated(NotifyCollectionChangedEventArgs args)
    => InvokeAsync(StateHasChanged);


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
}
