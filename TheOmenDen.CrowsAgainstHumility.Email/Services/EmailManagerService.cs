using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Identity;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models;
using TheOmenDen.Shared.Extensions;
#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Email.Services;

internal sealed class EmailManagerService : IDisposable, IAsyncDisposable, IEmailManagerService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IEmailConfigurationService _emailConfigurationService;

    public EmailManagerService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IEmailConfigurationService emailConfigurationService)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _emailConfigurationService = emailConfigurationService;

        StringBuilderPoolFactory<EmailManagerService>.Create(nameof(EmailManagerService));
    }

    public async Task BuildUserCreatedConfirmationMessageAsync(String emailAddress, string temporaryPassword,
        ApplicationUser applicationUser, String baseUri, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var callbackUrl = $"{baseUri}/Account/ConfirmEmailInternal?userId={applicationUser.Id}&token={token}";

        var prefixMessage = String.Empty;

        var sb = StringBuilderPoolFactory<EmailManagerService>.Get(nameof(EmailManagerService)) ?? new();

        if (String.IsNullOrWhiteSpace(prefixMessage))
        {
            sb.AppendLine(prefixMessage);
        }

        sb
            .AppendLine(
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}>Clicking Here</a>")
            .Append($"Your temporary password is {temporaryPassword}. Please Change this as soon as possible");

        await _emailSender.SendEmailAsync(
            emailAddress,
            "Confirm Your Email",
            sb.ToString());
    }

    public async Task BuildRegistrationConfirmationEmailAsync(String emailAddress, ApplicationUser user, String baseUri, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var callbackUrl = $"{baseUri}Account/ConfirmEmailInternal?userId={user.Id}&token={token}";

        var configuration = await _emailConfigurationService.GetEmailConfigurationAsync(cancellationToken);

        var prefixedConfirmationMessage = !String.IsNullOrWhiteSpace(configuration.RegistrationApprovalMessage)
            ? configuration.RegistrationApprovalMessage
            : String.Empty;

        await _emailSender.SendEmailAsync(
        emailAddress,
        "Confirm your email",
        $"{prefixedConfirmationMessage}Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>"
        );
    }

    public async Task BuildForgotPasswordEmailConfirmationAsync(String emailAddress, ApplicationUser applicationUser,
        string baseUri, CancellationToken cancellationToken = default)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);

        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var callbackUrl = $"{baseUri}reset-password?email={emailAddress}&token={token}";

        await _emailSender.SendEmailAsync(
            emailAddress,
            "Reset Password",
            $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>"
        );
    }

    public void Dispose()
    {
        if (StringBuilderPoolFactory<EmailManagerService>.Exists(nameof(EmailManagerService)))
        {
            StringBuilderPoolFactory<EmailManagerService>.Remove(nameof(EmailManagerService));
        }

        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        if (StringBuilderPoolFactory<EmailManagerService>.Exists(nameof(EmailManagerService)))
        {
            StringBuilderPoolFactory<EmailManagerService>.Remove(nameof(EmailManagerService));
        }

        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
