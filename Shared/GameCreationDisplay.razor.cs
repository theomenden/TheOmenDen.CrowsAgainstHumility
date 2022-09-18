using Microsoft.EntityFrameworkCore;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Shared
{
    public partial class GameCreationDisplay : ComponentBase
    {
        [Inject]
        public IDbContextFactory<CrowsAgainstHumilityContext> DbContextFactory { get; init; }

        private string _playerName = String.Empty;

        private string _playerToAdd = String.Empty;

        private List<String> _playerNames = new (10);

        private Pack[] _packs = Array.Empty<Pack>();

        private List<Pack> _chosenPacks = new (10);

        private List<String> _chosenPackTexts = new(10);

        protected override async Task OnInitializedAsync()
        {
            await using var context = await DbContextFactory.CreateDbContextAsync();

            _packs = await context.Packs
                .ToArrayAsync();
        }

        private Task AddPlayerToList()
        {
            _playerNames.Add(_playerToAdd);

            return Task.CompletedTask;
        }

        private Task AddOfficialPacks()
        {
            var officialPacks = _packs.Where(p => p.IsOfficialPack).ToArray();

            _chosenPacks.AddRange(officialPacks);

            return Task.CompletedTask;
        }

        private Task AddRandomPacks()
        {
            var packsToAdd = _packs.

            return Task.CompletedTask;
        }

        private Task RemoveAllPacks()
        {
            _chosenPacks.Clear();

            return Task.CompletedTask;
        }
    }
}
