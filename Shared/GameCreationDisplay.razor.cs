using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.Immutable;
using TheOmenDen.CrowsAgainstHumility.Data.Contexts;
using TheOmenDen.Shared.Extensions;
using TheOmenDen.Shared.Utilities;

namespace TheOmenDen.CrowsAgainstHumility.Shared
{
    public partial class GameCreationDisplay : ComponentBase
    {
        [Inject]
        public IDbContextFactory<CrowsAgainstHumilityContext> DbContextFactory { get; init; }

        private string _playerName = String.Empty;

        private string _playerToAdd = String.Empty;

        private readonly List<String> _playerNames = new (10);

        private ImmutableList<Pack> _packs = ImmutableList.Create<Pack>();

        private Dictionary<Guid,Pack> _packsToChoose = new(10);

        private List<string> _chosenPackTexts = new(10);

        protected override async Task OnInitializedAsync()
        {
            await using var context = await DbContextFactory.CreateDbContextAsync();

            _packs = context.Packs
                .ToImmutableList();
        }

        private Task AddPlayerToList()
        {
            _playerNames.Add(_playerToAdd);

            return Task.CompletedTask;
        }

        private Task AddOfficialPacks()
        {
            var officialPacks = _packs.Where(p => p.IsOfficialPack).ToArray();

            foreach (var pack in _packs.Where(p => p.IsOfficialPack))
            {
                _packsToChoose.TryAdd(pack.Id, pack);
            }

            return Task.CompletedTask;
        }

        private Task AddRandomPacks()
        {
            foreach(var pack in _packs.GetRandomElements(5))
            {
                _packsToChoose.TryAdd(pack.Id,pack);
            }

            return Task.CompletedTask;
        }

        private Task RemoveAllPacks()
        {
            _packsToChoose.Clear();

            return Task.CompletedTask;
        }
    }
}
