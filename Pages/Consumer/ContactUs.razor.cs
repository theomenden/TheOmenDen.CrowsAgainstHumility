using Microsoft.AspNetCore.Identity.UI.Services;
using TheOmenDen.CrowsAgainstHumility.Services;

namespace TheOmenDen.CrowsAgainstHumility.Pages.Consumer;

public partial class ContactUs: ComponentBase
{
    private string _token = String.Empty;



    [Inject] private IRecaptchaService ReCaptchaService { get; init; }
    [Inject] private IEmailSender EmailSender { get; init; }

    private string _email = String.Empty;
    private string _firstName = String.Empty;
    private string _lastName = String.Empty;
    private string _emailContent = String.Empty;

    private async Task SubmitContactFormAsync()
    {
        var token = await ReCaptchaService.GenerateCaptchaTokenAsync("submit");

        if (await ReCaptchaService.VerifyCaptchaAsync(token))
        {
            var senderName = $"{_firstName} {_lastName}";

            await EmailSender.SendEmailAsync(_email,$"Support Request- {senderName}",_emailContent);
        }
    }
}
