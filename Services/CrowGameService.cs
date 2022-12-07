using AngleSharp.Dom;
using Microsoft.AspNetCore.SignalR.Client;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Store;

namespace TheOmenDen.CrowsAgainstHumility.Services;

public sealed class CrowGameService : IDisposable
{
    #region Injections
    private readonly HubConnection _hubConnection;
    private readonly NavigationManager _navigationManager;
    #endregion
    #region Fields
    private readonly CrowGameState _gameState = new();
    private readonly List<RoomStateDto> _rooms = Enumerable.Empty<RoomStateDto>().ToList();
    private List<CrowGamePlayer> _players = Enumerable.Empty<CrowGamePlayer>().ToList();
    private RoomStateDto? _currentRoomState = null;
    private Guid? _playerGuid = null;
    #endregion
    #region Event Handlers
    public event EventHandler? RoomListChanged;
    public event EventHandler? PlayerListChanged;
    public event EventHandler? RoomSettingsChanged;
    public event EventHandler? GameStarted;
    public event EventHandler<PlayerWhiteCardChoiceEventArgs>? CardCzarChoiceStarted;
    public event EventHandler<(PlayerDto player, Int32 timeout)>? PlayerCardChoiceStarted;
    public event EventHandler<PlayerDto>? PlayerWhiteCardChosen;
    public event EventHandler<(List<PlayerScore> scores, Int32 timeout)>? GameScores;
    #endregion

    public CrowGameService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri("/game"))
            .Build();

        _ = Initializer();
    }

    #region Properties
    public IEnumerable<RoomStateDto> Rooms => _rooms;
    public IEnumerable<CrowGamePlayer> Players => _players;
    public RoomStateDto? RoomState => _currentRoomState;
    public CrowGameState GameState => _gameState;
    public Guid? Guid => _playerGuid;
    #endregion

    #region Public Methods
    public async Task<Boolean> CreateRoomAsync(String roomName, CrowRoomSettings roomSettings, CancellationToken cancellationToken = default) =>
    await _hubConnection.InvokeAsync<bool>("CreateRoom", roomName, roomSettings, cancellationToken);

    public async Task<Boolean> JoinRoomAsync(string roomName, string roomCode, CancellationToken cancellationToken = default)
    {
        var roomState =
            await _hubConnection.InvokeAsync<RoomStateDto>("JoinRoom", roomName, roomCode, cancellationToken);

        if (roomState is null)
        {
            _players = new List<CrowGamePlayer>(10);
            PlayerListChanged?.Invoke(this, EventArgs.Empty);
            _currentRoomState = null;
            RoomSettingsChanged?.Invoke(this, EventArgs.Empty);
            return false;
        }

        _players = roomState.Players.Select(p => new CrowGamePlayer(p)).ToList();
        PlayerListChanged?.Invoke(this, EventArgs.Empty);
        _currentRoomState = roomState;
        RoomSettingsChanged?.Invoke(this, EventArgs.Empty);

        switch (roomState.IsGameInProgress)
        {
            case true:
                _navigationManager.NavigateTo("/room");
                break;
            case false:
                _navigationManager.NavigateTo("/waitingRoom");
                break;
        }

        return true;
    }

    public async Task<RoomStateDto> TryReconnectAsync(String userName, Guid connectionGuid, CancellationToken cancellationToken = default)
    {
        var reconnectionState = await _hubConnection.InvokeAsync<ReconnectionStateDto>("TryReconnect", userName, connectionGuid, cancellationToken);

        if (reconnectionState.RoomState is not null)
        {
            _playerGuid = reconnectionState.PlayerId;
            _players = reconnectionState.RoomState.Players.Select(p => new CrowGamePlayer(p)).ToList();
            PlayerListChanged?.Invoke(this, EventArgs.Empty);
            _currentRoomState = reconnectionState.RoomState;
            RoomSettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        return RoomState;
    }

    public async Task<Guid> SetPlayerNameAsync(String userName, CancellationToken cancellationToken = default)
    {
        var playerGuids = await _hubConnection.InvokeAsync<PlayerConnections>("SetPlayerName", userName, cancellationToken);

        _playerGuid = playerGuids.PlayerGuid;

        return playerGuids.ConnectionGuid;
    }

    public async Task<Boolean> LeaveRoomAsync(CancellationToken cancellationToken = default)
    {
        if (!await _hubConnection.InvokeAsync<bool>("LeaveRoom", cancellationToken))
        {
            return false;
        }

        var newRooms = await _hubConnection.InvokeAsync<List<RoomStateDto>>("GetRooms", cancellationToken);
        _rooms.Clear();
        AddRooms(newRooms);
        return true;
    }

    public Task<Boolean> StartGameAsync(CancellationToken cancellationToken = default) => _hubConnection.InvokeAsync<bool>("StartGame", cancellationToken);

    public Task SetRoomSettingsAsync(CrowRoomSettings roomSettings, CancellationToken cancellationToken = default)
    {
        if (_currentRoomState is not null)
        {
            _currentRoomState.RoomSettings = roomSettings;
        }

        return _hubConnection.InvokeAsync("SetRoomSettings", cancellationToken);
    }

    public void Dispose() => _gameState?.TurnTimer?.Dispose();
    #endregion
    #region Private Methods
    private void AddRooms(IEnumerable<RoomStateDto> newRooms)
    {
        _rooms.AddRange(newRooms);
        RoomListChanged?.Invoke(this, EventArgs.Empty);
    }

    private void AddRoom(RoomStateDto room)
    {
        _rooms.Add(room);
        RoomListChanged?.Invoke(this, EventArgs.Empty);
    }

    private void RemoveRoom(RoomStateDto room)
    {
        var room2 = _rooms.FirstOrDefault(r => room.RoomName.Equals(r.RoomName));

        if (room2 is null)
        {
            return;
        }

        _rooms.Remove(room2);
        RoomListChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task Initializer()
    {
        InitializeServerCallbacks();

        await _hubConnection.StartAsync();

        var newRooms = await _hubConnection.InvokeAsync<List<RoomStateDto>>("GetRooms");

        AddRooms(newRooms);
    }

    private void InitializeServerCallbacks()
    {
        _hubConnection.On<PlayerDto>("PlayerJoined", p =>
        {
            var player = new CrowGamePlayer(p);

            if (_players.Contains(player))
            {
                return;
            }

            _players.Add(player);
            PlayerListChanged?.Invoke(this, EventArgs.Empty);
        });

        _hubConnection.On<PlayerDto>("PlayerConnectionStatusChanged", playerDto =>
        {
            var player = _players.SingleOrDefault(p => p.Id.Equals(playerDto.Id));

            if (player is null)
            {
                return;
            }

            player.IsConnected = playerDto.IsConnected;
            PlayerListChanged?.Invoke(this, EventArgs.Empty);

            var playerName = playerDto.Name;
            var playerStatusMessage = $"{playerName}{(playerDto.IsConnected ? " reconnected." : " disconnected.")}";
            
            _gameState.AddChatMessage(new(CrowChatMessageType.GameFlow, playerStatusMessage, playerName));
        });

        _hubConnection.On<PlayerDto>("PlayerLeft", p =>
        {
            var player = new CrowGamePlayer(p);

            if (!_players.Contains(player))
            {
                return;
            }

            _players.Remove(player);
            PlayerListChanged?.Invoke(this, EventArgs.Empty);

            var playerName = p.Name;

            _gameState.AddChatMessage(new(CrowChatMessageType.GameFlow, $"{playerName} left the game", p.Name));
        });

        _hubConnection.On<RoomStateDto>("RoomCreated", AddRoom);
        _hubConnection.On<RoomStateDto>("RoomDeleted", RemoveRoom);
        _hubConnection.On<RoomStateDto>("RoomSTateChanged", state =>
        {
            if (_currentRoomState is not null && state.RoomName.Equals(_currentRoomState.RoomName))
            {
                _currentRoomState = state;
            }

            var room = _rooms.FirstOrDefault(r => r.RoomName.Equals(state.RoomName));

            if (room is not null)
            {
                room.IsGameInProgress = state.IsGameInProgress;
                room.RoomSettings = state.RoomSettings;
                room.Players = state.Players;
            }

            RoomSettingsChanged?.Invoke(this, EventArgs.Empty);
        });

        _hubConnection.On("GameStarted", () =>
        {
            foreach (var p in _players)
            {
                p.AwesomePoints = 2;
                p.DisplayPosition = null;
            }

            GameStarted?.Invoke(this, EventArgs.Empty);
        });

        _hubConnection.On<Int32, Int32, CrowChatMessage>("RoundStarted", (currentRound, roundCount, chatMessage) =>
            _gameState.NewRoundStarted(currentRound, roundCount, chatMessage));

        _hubConnection.On<CrowChatMessage>("ChatMessage", cm => _gameState.AddChatMessage(cm));

        _hubConnection.On<PlayerDto, Int32, WhiteCard, CrowChatMessage>("WhiteCardChosen",
            (player, timeRemaining, whiteCard, chatMessage) =>
            {
                PlayerWhiteCardChosen?.Invoke(this, player);
                _gameState.TurnTimer.SetTime(timeRemaining);

                if (whiteCard is not null)
                {
                    _gameState.ChosenWhiteCard(whiteCard);
                }

                if (chatMessage is not null)
                {
                    _gameState.AddChatMessage(chatMessage);
                }
            });

        _hubConnection.On<List<PlayerScore>, WhiteCard, Int32>("TurnScores", (scores, whiteCard, timeout) => _gameState.TurnScoresReceived(scores, whiteCard, timeout));

        _hubConnection.On<List<PlayerScore>>("UpdateTotalScores", UpdatePlayerScores);

        _hubConnection.On<List<PlayerScore>, int>("GameScores",
            (scores, timeout) => GameScores?.Invoke(this, (scores, timeout)));

        _hubConnection.On("GameEnded", () => _navigationManager.NavigateTo("/waitingRoom"));
    }

    private void UpdatePlayerScores(List<PlayerScore> scores)
    {
        foreach (var ps in scores)
        {
            foreach (var p in _players.Where(p => p.Id.Equals(ps.Player.Id)))
            {
                p.AwesomePoints = ps.Score;
            }
        }

        UpdatePlayerDisplayPositions();
    }

    private void UpdatePlayerDisplayPositions()
    {
        var sortedPlayers = new List<CrowGamePlayer>(_players);

        sortedPlayers.Sort((p1, p2) => p2.CompareTo(p1));

        for (var i = 0; i < sortedPlayers.Count; i++)
        {
            if (i > 0 && sortedPlayers[i].AwesomePoints == sortedPlayers[i - 1].AwesomePoints)
            {
                sortedPlayers[i].DisplayPosition = sortedPlayers[i - 1].DisplayPosition;
                continue;
            }

            sortedPlayers[i].DisplayPosition = i + 1;
        }
    }
    #endregion

}