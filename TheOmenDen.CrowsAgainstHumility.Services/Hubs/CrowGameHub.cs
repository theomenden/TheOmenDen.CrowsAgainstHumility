using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.CrowsAgainstHumility.Services.CrowGameBuilder;

namespace TheOmenDen.CrowsAgainstHumility.Services.Hubs;
public sealed class CrowGameHub : Hub
{
    public const string HubUrl = "/crowGameHub";

    private readonly IGameRoomManager _roomManager;

    public CrowGameHub(IGameRoomManager roomManager)
    {
            _roomManager = roomManager ?? throw new ArgumentNullException(nameof(roomManager));
    }

    [HubMethodName(CrowGameHubConnectorService.JoinRoomMethodName)]
    public async Task JoinRoom(Guid roomId, Player player)
    {
        var roomState = await _roomManager.AddPlayerToRoomAsync(roomId, player, Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, roomState.Code);
        await Clients.Groups(roomState.Code).SendAsync(CrowGameHubConnectorService.RoomUpdatedMethodName, roomState);
    }

    [HubMethodName(CrowGameHubConnectorService.JoinRoomByCodeMethodName)]
    public async Task JoinRoomByCode(Guid roomId, Player player)
    {
        var roomState = await _roomManager.AddPlayerToRoomAsync(roomId, player, Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, roomState.Code);
        await Clients.Groups(roomState.Code).SendAsync(CrowGameHubConnectorService.RoomUpdatedMethodName, roomState);
    }

    [HubMethodName(CrowGameHubConnectorService.UpdateRoomMethodName)]
    public async Task UpdateRoom(RoomOptions roomOptions)
    {
        var roomState = await _roomManager.UpdateRoomAsync(roomOptions, Context.ConnectionId);

        if (!String.IsNullOrWhiteSpace(roomState?.Code))
        {
            await Clients.Groups(roomState.Code).SendAsync(CrowGameHubConnectorService.RoomUpdatedMethodName, roomState);
        }
    }

    [HubMethodName(CrowGameHubConnectorService.PlayWhiteCardMethodName)]
    public async Task PlayWhiteCard(WhiteCard card)
    {
        var roomState = await _roomManager.PlayWhiteCardAsync(card, Context.ConnectionId);

        if (!String.IsNullOrWhiteSpace(roomState?.Code))
        {
            await Clients.Groups(roomState.Code).SendAsync(CrowGameHubConnectorService.RoomUpdatedMethodName, roomState);
        }
    }

    [HubMethodName(CrowGameHubConnectorService.NextBlackCardMethodName)]
    public async Task NextBlackCard(Guid roomId, BlackCard blackCard)
    {
        var roomState = await _roomManager.NextBlackCardAsync(roomId, blackCard);

        if (!String.IsNullOrWhiteSpace(roomState?.Code))
        {
            await Clients.Groups(roomState.Code).SendAsync(CrowGameHubConnectorService.RoomUpdatedMethodName, roomState);
        }
    }

    [HubMethodName(CrowGameHubConnectorService.ResetRoomMethodName)]
    public async Task ResetRoom()
    {
        var roomState = await _roomManager.ResetGameBoardAsync(Context.ConnectionId);

        if (!String.IsNullOrWhiteSpace(roomState?.Code))
        {
            await Clients.Groups(roomState.Code).SendAsync(CrowGameHubConnectorService.RoomUpdatedMethodName, roomState);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var room = await _roomManager.DisconnectAsync(Context.ConnectionId);

        if (!String.IsNullOrWhiteSpace(room?.Code))
        {
            await Clients.Groups(room.Code).SendAsync(CrowGameHubConnectorService.RoomUpdatedMethodName, room);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
