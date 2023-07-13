using TheOmenDen.CrowsAgainstHumility.Azure.SignalR.Clients;

namespace TheOmenDen.CrowsAgainstHumility.Components.Game;

public partial class CrowGameLogComponent: ComponentBase
{
    [Parameter] public ICrowGameHubClient HubClient { get; set; }
    [Parameter] public EventCallback<ICrowGameHubClient> OnHubClientChanged { get; set; }
    
    private List<GameMessage> _gameLog = new (50);

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        HubClient.OnLogMessageReceived(logMessage =>
        {
            _gameLog.Add(logMessage);
            StateHasChanged();
        });
    }

    private DateTime ToLocalTime(DateTime time) => TimeZoneInfo.ConvertTimeFromUtc(time, TimeZoneInfo.Local);
}
