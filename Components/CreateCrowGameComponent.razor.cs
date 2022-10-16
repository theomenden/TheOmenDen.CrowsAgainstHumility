using Blazorise;
using Blazorise.Components;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.CrowsAgainstHumility.Services.Interfaces;
using TheOmenDen.Shared.Utilities;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class CreateCrowGameComponent : ComponentBase
{
    private static string ModalTitle = "Player Does Not Exist"; 

    [Parameter] public Boolean IsVisible { get; init; } = false;

    [Inject]
    public IDbContextFactory<CrowsAgainstHumilityContext> DbContextFactory { get; init; }

    [Inject]
    public IPlayerVerificationService PlayerVerificationService { get; init; }

    [Inject]
    public IMessageService MessageService { get; init; }

    private Int32 _totalCardsInPool = 0;

    private string _playerName = String.Empty;

    private string _playerToAdd = String.Empty;

    private readonly List<User> _players = new(10);

    private List<Pack> _packs = new(300);

    private readonly Dictionary<Guid, Pack> _packsToChoose = new(10);

    private List<Pack> _chosenPacks = new(10);

    private List<string> _chosenPackTexts = new(10);

    private bool _isIndicatorVisible;

    private async Task OnHandleReadData(AutocompleteReadDataEventArgs autocompleteReadDataEventArgs)
    {
        if (!autocompleteReadDataEventArgs.CancellationToken.IsCancellationRequested)
        {
            await using var context = await DbContextFactory.CreateDbContextAsync(autocompleteReadDataEventArgs.CancellationToken);

            var packsQuery = context.Packs
                .Include(p => p.WhiteCards)
                .Include(p => p.BlackCards)
                .AsQueryable();

            if (!String.IsNullOrWhiteSpace(autocompleteReadDataEventArgs.SearchValue))
            {
                packsQuery = packsQuery
                    .Where(p => p.Name.StartsWith(autocompleteReadDataEventArgs.SearchValue));
            }

            _packs = await packsQuery.ToListAsync(autocompleteReadDataEventArgs.CancellationToken);
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

    private async Task OnPlayersUpdated(NotifyCollectionChangedEventArgs args)
    {
        
        await InvokeAsync(StateHasChanged);
    }

    private async Task AddOfficialPacks()
    {
        await using var context = await DbContextFactory.CreateDbContextAsync();

        _isIndicatorVisible = true;

        await foreach (var pack in context.Packs
                     .Where(p => p.IsOfficialPack)
                     .Include(p => p.BlackCards)
                     .Include(p => p.WhiteCards)
                     .AsAsyncEnumerable())
        {
            if (_packsToChoose.TryAdd(pack.Id, pack))
            {
                pack.WhiteCards.TryGetNonEnumeratedCount(out var whiteCardCount);
                _totalCardsInPool += whiteCardCount;
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
                pack.WhiteCards.TryGetNonEnumeratedCount(out var whiteCardCount);

                _totalCardsInPool += whiteCardCount;
            }
            await InvokeAsync(StateHasChanged);
        }
        _isIndicatorVisible = false;
    }

    private async Task AddRandomPacks()
    {
        await using var context = await DbContextFactory.CreateDbContextAsync();

        _isIndicatorVisible = true;

        var skippedRows = ThreadSafeRandom.Global.Next(0, 130);
        var rowsToTake = ThreadSafeRandom.Global.Next(5, 10);

        await foreach (var pack in context.Packs
                     .Skip(skippedRows)
                     .Take(rowsToTake)
                     .Include(p => p.BlackCards)
                     .Include(p => p.WhiteCards)
                     .AsAsyncEnumerable())
        {
            if (_packsToChoose.TryAdd(pack.Id, pack))
            {
                pack.WhiteCards.TryGetNonEnumeratedCount(out var whiteCardCount);
                _totalCardsInPool += whiteCardCount;
            }
            await InvokeAsync(StateHasChanged);
        }

        _isIndicatorVisible = false;
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
