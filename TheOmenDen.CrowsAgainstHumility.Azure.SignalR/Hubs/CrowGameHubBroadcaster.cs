using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;
using TheOmenDen.CrowsAgainstHumility.Core.Results;
using TheOmenDen.CrowsAgainstHumility.Core.Transformation.Mappers;

namespace TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Hubs;
internal sealed class CrowGameHubBroadcaster : ICrowGameHubBroadcaster
{
    private static readonly Lazy<CrowGameServerMapper> _serverMapper = new(() => new());
    private static readonly Lazy<PlayerMapper> _playerMapper = new(() => new());
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IHubContext<CrowGameHub> _hubContext;

    public CrowGameHubBroadcaster(
        IDateTimeProvider? dateTimeProvider,
        ICrowGameEngine engine,
        IHubContext<CrowGameHub> hubContext)
    {
        _dateTimeProvider = dateTimeProvider ?? DateTimeProvider.Default;
        _hubContext = hubContext;
        
        engine.RoomUpdated += OnRoomUpdated;
        engine.RoomCleared += OnRoomCleared;
        engine.PlayerKicked += OnPlayerKicked;
        engine.LogUpdated += OnLogUpdated;
    }

    private void OnRoomUpdated(object? sender, RoomUpdatedEventArgs e)
        => _hubContext.Clients
            .Group(e.ServerId.ToString())
            .SendAsync(BroadcastChannels.Updated.ToString(), _serverMapper.Value.ServerToServerViewModel(e.UpdatedServer));

    private void OnRoomCleared(object? sender, RoomClearedEventArgs e)
        => _hubContext.Clients
            .Group(e.ServerId.ToString())
            .SendAsync(BroadcastChannels.Clear.ToString());

    private void OnPlayerKicked(object? sender, PlayerKickedEventArgs e)
        => _hubContext.Clients
            .Group(e.ServerId.ToString())
            .SendAsync(BroadcastChannels.Kicked.ToString(), _playerMapper.Value.PlayerToPlayerViewModel(e.KickedPlayer));

    private void OnLogUpdated(object? sender, LogUpdatedEventArgs e)
    {
        var utcCurrent = _dateTimeProvider.UtcNow;

        var logMessage = new GameMessage(MessageTypes.Empty, e.LogMessage, e.InitiatingPlayer,
            utcCurrent);

        _hubContext.Clients
            .Group(e.ServerId.ToString())
            .SendAsync(BroadcastChannels.Log.ToString(), logMessage);
    }
}
