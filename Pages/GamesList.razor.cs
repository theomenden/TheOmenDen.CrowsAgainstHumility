using TheOmenDen.CrowsAgainstHumility.Services;
using TheOmenDen.CrowsAgainstHumility.Services.Authentication;
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
        if (!StringBuilderPoolFactory<GameCodeGenerator>.Exists(nameof(GameCodeGenerator)))
        {
            return GameCodeGenerator.GenerateGameCode();
        }

        var sb =  StringBuilderPoolFactory<GameCodeGenerator>.Get(nameof(GameCodeGenerator));

        sb!.Clear();

        return GameCodeGenerator.GenerateGameCode();
    }

    private bool ShowCreateCrowGame() => !String.IsNullOrWhiteSpace(_gameName) && _gameName?.Length > 3;

    public ValueTask DisposeAsync()
    {
        _gameName = String.Empty;
        _gameCode = String.Empty;

        if (StringBuilderPoolFactory<GameCodeGenerator>.Exists(nameof(GameCodeGenerator)))
        {
            StringBuilderPoolFactory<GameCodeGenerator>.Remove(nameof(GameCodeGenerator));
        }

        return ValueTask.CompletedTask;
    }
}