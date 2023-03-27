namespace TheOmenDen.CrowsAgainstHumility.Core.Results;
public sealed class LogMessage
{
    public string User { get; set; } = String.Empty;
    public string Message { get; set; } = String.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
