using CommunityToolkit.Mvvm.ComponentModel;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Services;

namespace TheOmenDen.CrowsAgainstHumility.ViewModels;

public partial class RoomViewModel : ViewModelBase
{
    #region Private Members
    private readonly ICookie _cookies;
    private readonly IClipboardService _clipboardService;
    private readonly IUserToPlayerProcessingService _userService;
    private readonly ICrowGameHubConnectorService _crowGameHubConnectorService;
    private readonly HttpClient _httpClient;
    private CancellationTokenSource? _timerCancellationTokenSource;
    #endregion
    #region Constructors
    public RoomViewModel(ICookie cookies, IClipboardService clipboardService, ICrowGameHubConnectorService crowGameHubConnectorService, IUserToPlayerProcessingService userService, HttpClient httpClient)
    {
        _cookies = cookies;
        _clipboardService = clipboardService;
        _crowGameHubConnectorService = crowGameHubConnectorService;
        _httpClient = httpClient;
        _userService = userService;
        _crowGameHubConnectorService.RoomStateUpdated += RoomStateUpdated;
    }
    #endregion
    #region Observable Properties

    [ObservableProperty] 
    private int _selectedRoleId;

    [ObservableProperty] 
    private bool _showPlayedWhiteCards;

    [ObservableProperty] 
    private bool _previewCards = true;

    [ObservableProperty] 
    private bool _autoShowPlayedWhiteCards;

    [ObservableProperty] 
    private DateTime? _whiteCardPlayTime;

    [ObservableProperty]
    private RoomStateDto? _roomStateDto;

    [ObservableProperty]
    private Guid _currentUserId;

    [ObservableProperty]
    private string? _username;

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private bool _isNameModalOpen;

    [ObservableProperty]
    public string _copyButtonText = "Copy Invitation Link ";

    [ObservableProperty]
    public string _copyButtonIcon = "fa-duotone fa-copy";

    [ObservableProperty]
    public ClipboardResults _clipboardResult = ClipboardResults.NotCopied;
    #endregion
    #region Public Properties
    public String? RoomCode { get; set; }

    public bool IsCardTsar => CurrentPlayer?.IsCardCzar ?? false;

    public bool IsPlayer => CurrentPlayer?.GameRole == GameRoles.Player;

    public bool IsObserver => CurrentPlayer?.GameRole == GameRoles.Observer;

    public bool IsFacilitator => CurrentPlayer?.GameRole == GameRoles.Facilitator;

    public Player? CurrentPlayer => RoomStateDto?.Players.FirstOrDefault(p => p.Id == CurrentUserId);
    #endregion
    #region Async Partial methods
    private async Task OnWhiteCardsShownChanged(bool value)
    {
        if (_crowGameHubConnectorService.IsConnected)
        {
            await _crowGameHubConnectorService.UpdateRoomAsync(new RoomOptions
            {
                WhiteCardsShow = value
            });
        }
    }

    private async Task OnAutoWhiteCardsShownChanged(bool value)
    {
        if (_crowGameHubConnectorService.IsConnected)
        {
            await _crowGameHubConnectorService.UpdateRoomAsync(new RoomOptions
            {
                AutoShowWhiteCards = value
            });
        }
    }
    #endregion
    #region Public methods
    public async Task OnClickClipboard(string? uri)
    {
        if (!string.IsNullOrEmpty(uri))
        {
            await _clipboardService.CopyToClipboardAsync(uri);

            ClipboardResult = ClipboardResults.Copied;

            return;
        }

        ClipboardResult = ClipboardResults.NotCopied;
    }



    public async Task PlayWhiteCardAsync(WhiteCard whiteCard)
    {
        if (_crowGameHubConnectorService.IsConnected)
        {
            await _crowGameHubConnectorService.PlayWhiteCardAsync(whiteCard);
        }
    }

    public async Task PlayCardDialogAsync()
    {
        IsNameModalOpen = false;

        if (RoomStateDto is not null)
        {
            await _crowGameHubConnectorService.UpdatePlayerAsync(new PlayerOptions
            {

            });

            await SetCookiesAsync();
            return;
        }


        var name = String.IsNullOrWhiteSpace(Name) ? "User" : Name;
        var player = await _userService.GetPlayerByUsernameAsync(name);

        var (couldBeParsed, role) = GameRoles.TryParseFromValue(SelectedRoleId, GameRoles.Player);

        if (couldBeParsed)
        {
            player = player with { GameRole = role };
        }

        CurrentUserId = player?.Id ?? Guid.Empty;
        await _crowGameHubConnectorService.JoinRoomAsync(RoomStateDto?.RoomId ?? Guid.Empty, player);
    }

    public async Task ResetGameBoardAsync()
    {
        if (_crowGameHubConnectorService.IsConnected)
        {
            await _crowGameHubConnectorService.ResetGameBoardAsync();
        }
    }

    public override async Task OnInitializedAsync()
    {

        await base.OnInitializedAsync();

        await _crowGameHubConnectorService.OpenAsync();
        

        var cts = new CancellationTokenSource();
        Interlocked.Exchange(ref _timerCancellationTokenSource, cts)?.Cancel();

        _ = ProcessPlayingWhiteCardTimer(cts.Token);
    }

    #endregion
    #region Private Methods
    private void RoomStateUpdated(object? sender, RoomStateDto roomState)
    {
        _showPlayedWhiteCards = roomState.ShouldShowCards;
        _showPlayedWhiteCards = roomState.ShouldRevealCardsAutomatically;
        _whiteCardPlayTime = roomState.WhiteCardTurnTime;

        RoomStateDto = roomState;
    }

    private async Task ProcessPlayingWhiteCardTimer(CancellationToken cancellationToken)
    {
        using var playTimer = new PeriodicTimer(TimeSpan.FromSeconds(0.5));

        while (await playTimer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
        {
            if (_whiteCardPlayTime is not null)
            {
                base.NotifyStateChanged();
            }
        }
    }

    private async Task SetCookiesAsync()
    {
        if (!String.IsNullOrWhiteSpace(Name))
        {
            await _cookies.SetNameAsync(Name);
        }
        await _cookies.SetRoomAsync(RoomCode ?? String.Empty);
        await _cookies.SetRoleAsync(SelectedRoleId.ToString());
    }
    #endregion
}
