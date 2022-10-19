using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
public interface IEmailConfigurationService
{
    EmailConfiguration? GetEmailConfiguration();
    Task<EmailConfiguration> GetEmailConfigurationAsync(CancellationToken cancellationToken = default);

    Task<Boolean> UpdateConfigurationAsync(EmailConfiguration configuration,
        CancellationToken cancellationToken = default);
}
