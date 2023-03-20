using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Polly;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder;

internal sealed class CrowGameHubConnectorService: ICrowGameHubConnectorService
{
    #region Method Names
    public const string JoinRoomMethodName = "JoinRoom";
    public const string JoinRoomByCodeMethodName = "JoinRoomByCode";
    public const string PlayWhiteCardMethodName = "PlayWhiteCard";
    public const string NextBlackCardMethodName = "NextBlackCard"; 
    public const string UpdateRoomMethodName = "UpdateRoom";
    public const string UpdatePlayerMethodName = "UpdatePlayer";
    public const string ResetRoomMethodName = "ResetRoom";
    public const string RoomUpdatedMethodName = "RoomUpdated";
    #endregion
    #region EventHandlers
    public event EventHandler<RoomStateDto>? RoomStateUpdated;
    #endregion
    #region Private Members
    private readonly HubConnection _hubConnection;
    private readonly ILogger<CrowGameHubConnectorService> _logger;
    private readonly Uri _hubUrl;
    #endregion
    #region Constructors
    public CrowGameHubConnectorService(ILogger<CrowGameHubConnectorService> logger, Uri url)
    {
        _logger = logger;
        _hubUrl = url;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options => { })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<RoomStateDto>(RoomUpdatedMethodName, (roomState) => RoomStateUpdated?.Invoke(this, roomState));
    }
    #endregion
    #region Public Properties
    public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;
    #endregion
    #region Room State Methods
    public async Task OpenAsync()
    {
        var pauseBetweenFailures = TimeSpan.FromSeconds(20);
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(
            i => pauseBetweenFailures,
            (exception, timeSpan) =>
            {
                _logger.LogError("Error connecting to SignalR hub {HubUrl} - {@Ex}", _hubUrl, exception);
            }
            );

        await retryPolicy.ExecuteAsync(async () => await _hubConnection.StartAsync());
    }
    public Task JoinRoomAsync(Guid roomId, Player player) => _hubConnection.InvokeAsync(JoinRoomMethodName,  roomId, player);
    public Task JoinRoomByCodeAsync(String roomCode, Player player) => _hubConnection.InvokeAsync(JoinRoomByCodeMethodName,roomCode,player);
    public Task PlayWhiteCardAsync(WhiteCard cardPlayed) => _hubConnection.InvokeAsync(PlayWhiteCardMethodName,  cardPlayed);
    public Task NextBlackCardAsync(BlackCard blackCard) => _hubConnection.InvokeAsync(NextBlackCardMethodName, blackCard);
    public Task UpdateRoomAsync(RoomOptions roomOptions) => _hubConnection.InvokeAsync(UpdateRoomMethodName, roomOptions);
    public Task UpdatePlayerAsync(PlayerOptions playerOptions) => _hubConnection.InvokeAsync(UpdatePlayerMethodName, playerOptions); 
    public Task ResetGameBoardAsync() => _hubConnection.InvokeAsync(ResetRoomMethodName);
    #endregion
}
