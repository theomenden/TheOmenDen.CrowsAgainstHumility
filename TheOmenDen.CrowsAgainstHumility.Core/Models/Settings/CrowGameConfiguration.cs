using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Settings;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Settings;
public class CrowGameConfiguration : ICrowGameConfiguration
{
    #region CrowGame Server Options
    public int ClientInactivityCheckInterval { get; set; } = 60;
    public int WaitForMessageTimeout { get; set; } = 60;
    public string? RepositoryDirectory { get; set; }
    public int RepositoryPlayerListExpiration { get; set; } = 1200;
    public int ClientInactivityTimeout { get; set; } = 900;
    #endregion
    #region ICrowGame Implementations
    TimeSpan ICrowGameConfiguration.ClientInactivityCheckInterval 
        => TimeSpan.FromSeconds(ClientInactivityCheckInterval);

    TimeSpan ICrowGameConfiguration.WaitForMessageTimeout 
        => TimeSpan.FromSeconds(WaitForMessageTimeout);

    TimeSpan ICrowGameConfiguration.RepositoryPlayerListExpiration
        => TimeSpan.FromSeconds(RepositoryPlayerListExpiration); 

    TimeSpan ICrowGameConfiguration.ClientInactivityTimeout 
        => TimeSpan.FromSeconds(ClientInactivityTimeout);
    #endregion
}
