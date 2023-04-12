using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Engines;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
using TheOmenDen.CrowsAgainstHumility.Core.Results;

namespace TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Hubs;
internal sealed class CrowGameHubBroadcaster
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IHubContext<CrowGameHub> _hubContext;

    public CrowGameHubBroadcaster(
        IDateTimeProvider dateTimeProvider,
        ICrowGameEngine engine,
        IHubContext<CrowGameHub> hubContext)
    {
        _dateTimeProvider = dateTimeProvider;
        _hubContext = hubContext;
        
        engine.RoomUpdated += OnRoomUpdated;
        engine.RoomCleared += OnRoomCleared;
        engine.PlayerKicked += OnPlayerKicked;
        engine.LogUpdated += OnLogUpdated;
    }

    private void OnRoomUpdated(object? sender, RoomUpdatedEventArgs e)
        => _hubContext.Clients.Group(e.ServerId.ToString())
            .SendAsync(BroadcastChannels.Updated.ToString(), e.UpdatedServer.ToServerDto());

    private void OnRoomCleared(object? sender, RoomClearedEventArgs e)
        => _hubContext.Clients.Group(e.ServerId.ToString())
            .SendAsync(BroadcastChannels.Clear.ToString());

    private void OnPlayerKicked(object? sender, PlayerKickedEventArgs e)
        => _hubContext.Clients.Group(e.ServerId.ToString())
            .SendAsync(BroadcastChannels.Kicked.ToString(), e.KickedPlayer.MapToPlayer(false));

    private void OnLogUpdated(object? sender, LogUpdatedEventArgs e)
    {
        var utcCurrent = _dateTimeProvider.GetUtcNow();

        var logMessage = new LogMessage
        {
            User = e.InitiatingPlayer,
            Message = e.LogMessage,
            Timestamp = utcCurrent
        };

        _hubContext.Clients.Group(e.ServerId.ToString())
            .SendAsync(BroadcastChannels.Log.ToString(), logMessage);
    }
}
