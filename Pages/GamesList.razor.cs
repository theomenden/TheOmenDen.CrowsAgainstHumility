using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TheOmenDen.CrowsAgainstHumility.Hubs;
using TheOmenDen.CrowsAgainstHumility.Services;
using TheOmenDen.CrowsAgainstHumility.Store;
using TheOmenDen.CrowsAgainstHumility.Twitch.Services;
using TheOmenDen.Shared.Extensions;
using TheOmenDen.Shared.Utilities;

namespace TheOmenDen.CrowsAgainstHumility.Pages
{
    public partial class GamesList : ComponentBase, IAsyncDisposable
    {
        [Inject] public NavigationManager Navigation { get; init; }

        [Inject] public CrowGameService StateManager { get; init; }

        private string _gameName;

        private string _gameCode;

        private void InitializeNewGame()
        {
            StateManager.Game = new CrowGame()
            {
                Name = _gameName
            };
        }

        private void Start()
        {
            Navigation.NavigateTo("/play");
        }

        private string GenerateGameCode()
        {
            if (!StringBuilderPoolFactory<GameCodeGeneratorService>.Exists(nameof(GameCodeGeneratorService)))
            {
                return GameCodeGeneratorService.GenerateGameCode();
            }

            var sb =  StringBuilderPoolFactory<GameCodeGeneratorService>.Get(nameof(GameCodeGeneratorService));

            sb!.Clear();

            return GameCodeGeneratorService.GenerateGameCode();
        }

        public ValueTask DisposeAsync()
        {
            _gameName = String.Empty;
            _gameCode = String.Empty;
            return ValueTask.CompletedTask;
        }
    }
}
