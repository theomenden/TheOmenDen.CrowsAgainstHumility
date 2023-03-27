using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Engines;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;

namespace TheOmenDen.CrowsAgainstHumility.Services.Hubs;
internal sealed class CrowGameEventBroadcaster
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IHubContext<CrowGameHub> _hubContext;

    public CrowGameEventBroadcaster(IDateTimeProvider dateTimeProvider, ICrowGameEngine crowGameEngine, IHubContext<CrowGameHub> hubContext)
    {
        _hubContext = hubContext;
        _dateTimeProvider = dateTimeProvider;

        crowGameEngine.LogUpdated += OnLogUpdated;
        crowGameEngine.PlayerKicked += OnPlayerKicked;
        crowGameEngine.RoomCleared += OnRoomCleared;
        crowGameEngine.RoomUpdated += OnRoomUpdated;
    }

    private void OnRoomUpdated(object? sender, RoomUpdatedEventArgs roomUpdatedEventArgs)
        => _hubContext.Clients
            .Group(roomUpdatedEventArgs.ServerId.ToString())
            .SendAsync(BroadcastChannels.Updated.Name, roomUpdatedEventArgs.UpdatedServer.Map());

    private void OnRoomCleared(object? sender, RoomClearedEventArgs roomClearedEventArgs)
        => _hubContext.Clients
            .Group(roomClearedEventArgs.ServerId.ToString())
            .SendAsync(BroadcastChannels.Clear.Name);

    private void OnPlayerKicked(object? sender, PlayerKickedEventArgs playerKickedEventArgs)
        => _hubContext.Clients
            .Group(playerKickedEventArgs.ServerId.ToString())
            .SendAsync(BroadcastChannels.Kicked.Name, playerKickedEventArgs.KickedPlayer.Map(false));

    private void OnLogUpdated(object? sender, LogUpdatedEventArgs logUpdatedEventArgs)
    {
        var currentUtc = _dateTimeProvider.GetUtcNow();
        var logMessage = new LogMessage
        {
            User = logUpdatedEventArgs.InitiatingPlayer,
            Message = logUpdatedEventArgs.LogMessage,
            Timestamp = currentUtc
        };
        
        _hubContext.Clients
            .Group(logUpdatedEventArgs.ServerId.ToString())
            .SendAsync(BroadcastChannels.Log.Name, logMessage);
    }
}
