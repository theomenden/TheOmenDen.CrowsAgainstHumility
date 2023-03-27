using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models;

namespace TheOmenDen.CrowsAgainstHumility.Email.Services;
internal sealed class EmailSenderService : IEmailSender
{
    private readonly IEmailConfigurationService _emailConfigurationService;
    private EmailConfiguration? _emailConfiguration;

    public EmailSenderService(IEmailConfigurationService emailConfigurationService)
    {
        _emailConfigurationService = emailConfigurationService;
    }
    
    public async Task SendEmailAsync(String email, String subject, String htmlMessage)
    {
        _emailConfiguration = await _emailConfigurationService.GetEmailConfigurationAsync();

        await ExecuteAsync(_emailConfiguration.SendGridKey, subject, htmlMessage, email);
    }

    public async Task ExecuteAsync(String apiKey, String subject, String message, String email)
    {
        var client = new SendGridClient(apiKey);

        _emailConfiguration ??= await _emailConfigurationService.GetEmailConfigurationAsync();

        var sendGridMessage = new SendGridMessage
        {
            From = new EmailAddress(_emailConfiguration.EmailAddress, _emailConfiguration.EmailSenderName),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };

        sendGridMessage.AddTo(new EmailAddress(email));

        sendGridMessage.SetClickTracking(false, false);

        await client.SendEmailAsync(sendGridMessage);
    }
}
