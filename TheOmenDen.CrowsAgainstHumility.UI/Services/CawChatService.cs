using Microsoft.AspNetCore.SignalR.Client;

namespace TheOmenDen.CrowsAgainstHumility.Services;

internal sealed class CawChatService
{
    #region Injections

    private readonly HubConnection _hubConnection;
    private readonly NavigationManager _navigationManager;

    #endregion

    public CawChatService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri("/game"))
            .Build();

        _ = Initializer().ConfigureAwait(false);
    }

    private async Task Initializer()
    {
        InitializeServerCallbacks();

        if (_hubConnection.State is HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync().ConfigureAwait(false);
        }

        var newRooms = await _hubConnection.InvokeAsync<List<RoomStateDto>>("GetRooms");

    }
    private void InitializeServerCallbacks()
    {
        _hubConnection.On<CawChatMessage, string>("RecieveMessage", async (message, username) =>
        {

        });
    }
}
