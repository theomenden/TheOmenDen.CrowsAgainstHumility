namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;

public interface ICrowGameConfiguration
{
    TimeSpan ClientInactivityTimeout { get; }
    TimeSpan ClientInactivityCheckInterval { get; }
    TimeSpan WaitForMessageTimeout { get; }
    string? RepositoryDirectory { get; }
    TimeSpan RepositoryPlayerListExpiration { get; }
}
