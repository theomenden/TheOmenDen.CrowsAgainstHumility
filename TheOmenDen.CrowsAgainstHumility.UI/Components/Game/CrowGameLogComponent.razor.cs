using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Hubs;
using TheOmenDen.CrowsAgainstHumility.Core.Results;
using TheOmenDen.CrowsAgainstHumility.Utilities;

namespace TheOmenDen.CrowsAgainstHumility.Components.Game;

public partial class CrowGameLogComponent: ComponentBase
{
    [Parameter] public ICrowGameHubClient HubClient { get; set; }
    [Parameter] public EventCallback<ICrowGameHubClient> HubClientChanged {get; set; }

    private DropOutStack<LogMessage> _log = new (20);

    private TimeZoneInfo _localTimeZoneInfo = TimeZoneInfo.Local;

    protected override Task OnInitializedAsync()
    {
        HubClient.OnLogMessageReceived(logMessage =>
        {
            _log.Push(logMessage);
            StateHasChanged();
        });

        return base.OnInitializedAsync();
    }

    private DateTime ToLocalTime(DateTime time) => TimeZoneInfo.ConvertTimeFromUtc(time, _localTimeZoneInfo);
}
