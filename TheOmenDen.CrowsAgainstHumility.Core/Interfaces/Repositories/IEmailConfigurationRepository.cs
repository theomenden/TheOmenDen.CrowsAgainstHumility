using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
public interface IEmailConfigurationRepository
{
    EmailConfiguration? GetEmailConfiguration();
    Task<EmailConfiguration> GetEmailConfigurationAsync(CancellationToken cancellationToken = default);
    Task<Boolean> UpdateEmailConfigurationAsync(EmailConfiguration emailConfiguration, CancellationToken cancellationToken = default);
}
