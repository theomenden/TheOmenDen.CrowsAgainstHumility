using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Services;

internal sealed class EmailConfigurationService : IEmailConfigurationService
{
    private readonly IEmailConfigurationRepository _emailConfigurationRepository;
    
    public EmailConfigurationService(IEmailConfigurationRepository emailConfigurationRepository)
    {
        _emailConfigurationRepository = emailConfigurationRepository;
    }

    public EmailConfiguration? GetEmailConfiguration() => _emailConfigurationRepository.GetEmailConfiguration();

    public Task<EmailConfiguration> GetEmailConfigurationAsync(CancellationToken cancellationToken = default) => _emailConfigurationRepository.GetEmailConfigurationAsync(cancellationToken);

    public Task<Boolean> UpdateConfigurationAsync(EmailConfiguration configuration,
        CancellationToken cancellationToken = default)
        => _emailConfigurationRepository.UpdateEmailConfigurationAsync(configuration, cancellationToken);
}
