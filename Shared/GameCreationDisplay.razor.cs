using Microsoft.EntityFrameworkCore;
using Blazorise.Components;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.Shared.Extensions;
using TheOmenDen.Shared.Utilities;

namespace TheOmenDen.CrowsAgainstHumility.Shared
{
    public partial class GameCreationDisplay : ComponentBase
    {
        [Inject]
        public IDbContextFactory<CrowsAgainstHumilityContext> DbContextFactory { get; init; }

        private Int32 _totalCardsInPool = 0;

        private string _playerName = String.Empty;

        private string _playerToAdd = String.Empty;

        private readonly List<String> _playerNames = new(10);

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

                var packsQuery = context.Packs.AsQueryable();

                if (!String.IsNullOrWhiteSpace(autocompleteReadDataEventArgs.SearchValue))
                {
                    packsQuery = packsQuery
                        .Where(p => p.Name.StartsWith(autocompleteReadDataEventArgs.SearchValue));
                }

                _packs = await packsQuery
                    .Include(p => p.BlackCards)
                    .Include(p => p.WhiteCards)
                    .ToListAsync(autocompleteReadDataEventArgs.CancellationToken);
            }
        }

        private Task AddPlayerToList()
        {
            _playerNames.Add(_playerToAdd);

            return Task.CompletedTask;
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
                if(_packsToChoose.TryAdd(pack.Id, pack))
                {
                    pack.WhiteCards.TryGetNonEnumeratedCount(out var whiteCardCount);
                    _totalCardsInPool += whiteCardCount;
                }
                await InvokeAsync(StateHasChanged);
            }

            _isIndicatorVisible = false;
        }

        private Task AddSearchedPacks()
        {
            _isIndicatorVisible = true;
            foreach (var pack in _chosenPacks)
            {
                pack.WhiteCards.TryGetNonEnumeratedCount(out var whiteCardCount);

                _totalCardsInPool += whiteCardCount;
                _packsToChoose.TryAdd(pack.Id, pack);
            }
            _isIndicatorVisible = false;
            return Task.CompletedTask;
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
}
