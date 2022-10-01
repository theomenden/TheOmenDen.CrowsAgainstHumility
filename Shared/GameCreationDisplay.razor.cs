using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        private string _playerName = String.Empty;

        private string _playerToAdd = String.Empty;

        private readonly List<String> _playerNames = new (10);

        private List<Pack> _packs = new(300);

        private readonly Dictionary<Guid,Pack> _packsToChoose = new(10);

        private List<Pack> _chosenPacks = new (10);
        
        private List<string> _chosenPackTexts = new(10);
        
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
            
            foreach (var pack in context.Packs.Where(p => p.IsOfficialPack))
            {
                _packsToChoose.TryAdd(pack.Id, pack);
            }
        }

        private Task AddSearchedPacks()
        {
            foreach (var pack in _chosenPacks)
            {
                _packsToChoose.TryAdd(pack.Id, pack);
            }
            return Task.CompletedTask;
        }
        
        private async Task AddRandomPacks()
        {
            await using var context = await DbContextFactory.CreateDbContextAsync();

            var packs = context.Packs.Skip(ThreadSafeRandom.Global.Next(0, 130))
                .Take(ThreadSafeRandom.Global.Next(0, 40))
                .ToArray();

            Array.ForEach(packs.GetRandomElementsFromArray(5), pack =>
            {
                _packsToChoose.TryAdd(pack.Id,pack);
            });
        }

        private Task RemoveAllPacks()
        {
            _packsToChoose.Clear();
            _chosenPacks.Clear();
            _chosenPackTexts.Clear();
            
            return Task.CompletedTask;
        }
    }
}
