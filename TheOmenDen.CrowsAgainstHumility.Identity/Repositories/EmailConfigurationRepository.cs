using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Repositories;
using TheOmenDen.CrowsAgainstHumility.Identity.Contexts;

namespace TheOmenDen.CrowsAgainstHumility.Identity.Repositories;

internal sealed class EmailConfigurationRepository : IEmailConfigurationRepository
{
    private readonly IDbContextFactory<AuthDbContext> _dbContextFactory;

    public EmailConfigurationRepository(IDbContextFactory<AuthDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public EmailConfiguration? GetEmailConfiguration()
    {
        using var context = _dbContextFactory.CreateDbContext();

        return context.EmailConfigurations.FirstOrDefault();
    }

    public async Task<EmailConfiguration> GetEmailConfigurationAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var configuration = await context.EmailConfigurations.FirstOrDefaultAsync(cancellationToken);

        return configuration ?? await CreateEmailConfigurationAsync(cancellationToken);
    }

    public async Task<Boolean> UpdateEmailConfigurationAsync(EmailConfiguration emailConfiguration, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var configuration = await context.EmailConfigurations.FirstOrDefaultAsync(cancellationToken);

        if (configuration is null)
        {
            return false;
        }

        configuration.AnalyticsCode = emailConfiguration.AnalyticsCode;
        configuration.EmailAddress = emailConfiguration.EmailAddress;
        configuration.EmailSenderName = emailConfiguration.EmailSenderName;
        configuration.SendGridKey = emailConfiguration.SendGridKey;
        configuration.RegistrationApprovalMessage = emailConfiguration.RegistrationApprovalMessage;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<EmailConfiguration> CreateEmailConfigurationAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var newConfiguration = new EmailConfiguration
        {
            AnalyticsCode = null
        };

        await context.EmailConfigurations.AddAsync(newConfiguration, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return await context.EmailConfigurations.FirstOrDefaultAsync(cancellationToken);
    }
}
