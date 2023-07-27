using Blazorise;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using TheOmenDen.CrowsAgainstHumility.Azure.Clients;
using TheOmenDen.CrowsAgainstHumility.Core.Constants;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.ViewModels;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;
using TheOmenDen.CrowsAgainstHumility.Services;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class CreateCrowGameComponent : ComponentBase, IDisposable, IAsyncDisposable
{
    private const string ModalTitle = "Player Does Not Exist";
    #region Cascading Parameters
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
    #endregion
    #region Injected Services
    [Inject] private NavigationManager NavigationManager { get; init; }

    [Inject] private IJSRuntime JsRuntime { get; init; }

    [Inject] private IMessageService MessageService { get; init; }

    [Inject] private IModalService ModalService { get; init; }

    [Inject] private IPlayerVerificationService PlayerVerificationService { get; init; }

    [Inject] private UserManager<ApplicationUser> UserManager { get; init; }
    #endregion
    #region Private Pack Models
    private IEnumerable<Pack> _packs = Enumerable.Empty<Pack>();
    private List<Guid> _selectedPackIds = new(10);
    private List<string> _selectedPackNames = new(10);
    #endregion
    #region Private User Models
    private User _selectedPlayer;
    private string _playerToAdd = String.Empty;
    private readonly List<User> _players = new(10);
    #endregion
    #region Crow Game Input Models
    private ServerCreationResult _creationResult;
    private ICrowGameHubClient _hubClient;
    private CrowGameInputModel _gameInputModel = new();
    private int _roundtimeInMinutes = 15;
    private int _awesomePointsToWin = 10;
    private int _chosenGameRule = 1;
    #endregion
    #region public Properties
    public string CreatedLobbyAddress { get; set; } = String.Empty;
    public bool HasBeenCreated { get; set; }
    #endregion

    #region Pack Manipulation Methods
    private int GetEstimatedWhiteCardCount()
    => _selectedPackIds
            .Sum(selectedPackId =>
                _packs
                    .FirstOrDefault(f => f.Id == selectedPackId)
                    ?.WhiteCardsInPack ?? 0);

    private int GetEstimatedBlackCardCount()
        => _selectedPackIds
            .Sum(selectedPackId =>
                _packs
                    .FirstOrDefault(f => f.Id == selectedPackId)
                    ?.BlackCardsInPack ?? 0);

    private Task AddAllOfficialPacksAsync()
    {
        _selectedPackIds = _packs.Where(f => f.IsOfficialPack)
            .Select(f => f.Id)
            .ToList();
        return Task.CompletedTask;
    }

    private Task AddRandomPackAsync()
    {
        _selectedPackIds.Add(_packs.GetRandomElement().Id);
        return Task.CompletedTask;
    }

    private bool IsOfficialPack(string packName) => _packs.Any(p => p.Name.Equals(packName, StringComparison.OrdinalIgnoreCase) && p.IsOfficialPack);

    private Task Add5RandomPacksAsync()
    {
        var randomPacksToAdd = _packs
            .ExceptBy(_selectedPackIds, p => p.Id)
            .Select(p => p.Id)
            .GetRandomElements(5)
            .ToList();

        _selectedPackIds.AddRange(randomPacksToAdd);

        return Task.CompletedTask;
    }

    private Task ClearPacksAsync()
    {
        _selectedPackNames.Clear();
        _selectedPackIds.Clear();
        return Task.CompletedTask;
    }
    #endregion
    #region LifeCycle Methods
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        await using var connection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/hubs/crowGame"))
            .WithAutomaticReconnect()
            .Build();

        await connection.StartAsync();

        _hubClient = new CrowGameHubClient(connection);

        _gameInputModel.GameCode = LobbyCodeProvider.GenerateGameCodeFromComponent(nameof(CreateCrowGameComponent));
    }

    #endregion
    #region  Twitch Methods
    private async Task AddPlayerToListAsync()
    {
        var player = await PlayerVerificationService.CheckTwitchForUserAsync(_playerToAdd);

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

        await InvokeAsync(StateHasChanged);
    }

    private static String DeterminePlayerIconName(string broadcasterType)
    {
        if (String.IsNullOrWhiteSpace(broadcasterType))
        {
            return "fa-brands fa-twitch";
        }

        return String.Equals(broadcasterType, BroadcasterTypes.Affiliate.ToString(), StringComparison.InvariantCultureIgnoreCase)
            ? "fa-certificate"
            : "fa-badge-check";
    }


    private static string DeterminePlayerIconColor(string broadcasterType)
    {
        if (String.IsNullOrWhiteSpace(broadcasterType))
        {
            return "#fff";
        }

        return String.Equals(broadcasterType, BroadcasterTypes.Partner.ToString(), StringComparison.InvariantCultureIgnoreCase)
            ? $"color: #{TwitchColorCodes.MAIN_COLOR};"
            : $"color: #{TwitchColorCodes.ACCENT_COLOR};";
    }

    private Task RemovePlayerAsync(User player)
    {
        _players.Remove(player);
        return Task.CompletedTask;
    }

    #endregion
    private Task OnCancelClickedAsync()
    => ModalService.Hide();

    private async Task OnCrowGameCreated()
    {
        var creationResult = await _hubClient.CreateServer(_packs);
        _creationResult = creationResult;

        if (_creationResult.WasCreated)
        {
            var uri = NavigationManager.ToAbsoluteUri($"/lobby/{_creationResult.ServerId}");

            CreatedLobbyAddress = uri.AbsoluteUri;
            HasBeenCreated = true;
        }

        StateHasChanged();
    }

    private async Task CopyServerAddressToClipboard() => await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", CreatedLobbyAddress);

    private GameRules GetChosenGameRule() => GameRules.ParseFromValueOrDefault(_chosenGameRule, GameRules.Standard);

    #region Disposal Methods
    public void Dispose()
    {
        StringBuilderPoolFactory<GameCodeGenerator>.Remove(nameof(CreateCrowGameComponent));
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        StringBuilderPoolFactory<GameCodeGenerator>.Remove(nameof(CreateCrowGameComponent));
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
    #endregion
}
