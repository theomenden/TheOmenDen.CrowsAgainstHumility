namespace TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
public class LogUpdatedEventArgs: CrowGameEventArgs
{
    public LogUpdatedEventArgs(Guid serverId, string initiatingPlayer, string logMessage) 
        : base(serverId)
    {
        InitiatingPlayer = initiatingPlayer;
        LogMessage = logMessage;
    }
    public string InitiatingPlayer { get; set; }
    public string LogMessage { get; set; }
}
