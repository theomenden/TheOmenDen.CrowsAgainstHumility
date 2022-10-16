using TheOmenDen.CrowsAgainstHumility.Services;
using TheOmenDen.CrowsAgainstHumility.Twitch.Services;
using TheOmenDen.Shared.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Pages;

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

    private bool ShowCreateCrowGame() => !String.IsNullOrWhiteSpace(_gameName) && _gameName?.Length > 3;

    public ValueTask DisposeAsync()
    {
        _gameName = String.Empty;
        _gameCode = String.Empty;

        if (StringBuilderPoolFactory<GameCodeGeneratorService>.Exists(nameof(GameCodeGeneratorService)))
        {
            StringBuilderPoolFactory<GameCodeGeneratorService>.Remove(nameof(GameCodeGeneratorService));
        }

        return ValueTask.CompletedTask;
    }
}