namespace TheOmenDen.CrowsAgainstHumility.Core.Results;
public sealed class ServerCreationResult
{
    public bool WasCreated { get; set; }
    public Guid? ServerId { get; set; }
    public string? ValidationMessage { get; set; }
}
