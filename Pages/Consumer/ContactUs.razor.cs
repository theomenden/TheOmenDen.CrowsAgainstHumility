using Blazorise;
using Microsoft.AspNetCore.Identity.UI.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Constants;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Services;

namespace TheOmenDen.CrowsAgainstHumility.Pages.Consumer;

public partial class ContactUs : ComponentBase
{
    #region Private Members
    private Validations _validations;
    private ContactFormInputModel _formInput = new();
    private ContactUsSubjects _chosenSubject = ContactUsSubjects.General;
    private string _token = String.Empty;
    #endregion
    #region Injected Members
    [Inject] private IRecaptchaService ReCaptchaService { get; init; }
    [Inject] private IEmailSender EmailSender { get; init; }
    #endregion

    private async Task SubmitContactFormAsync()
    {
        if (!(await _validations.ValidateAll()))
        {
            return;
        }

        var token = await ReCaptchaService.GenerateCaptchaTokenAsync("submit");

        if (await ReCaptchaService.VerifyCaptchaAsync(token))
        {
            var emailSubject = String.Empty;

            _chosenSubject
                .When(ContactUsSubjects.Partner)
                    .Then(() => emailSubject = String.Format(ContactEmailSubjects.Partner, _formInput.Subject))
                .When(ContactUsSubjects.Support)
                    .Then(() => emailSubject = String.Format(ContactEmailSubjects.Support, _formInput.Subject))
                .When(ContactUsSubjects.Unsubscribe)
                    .Then(() => emailSubject = String.Format(ContactEmailSubjects.Unsubscribe, _formInput.Subject))
                .DefaultCondition(() => emailSubject = String.Format(ContactEmailSubjects.General, _formInput.Subject));

            await EmailSender.SendEmailAsync(_formInput.Email, emailSubject, _formInput.Body);
        }
    }

    private async Task ClearContactFormAsync()
    {
        _formInput = new();
        await _validations.ClearAll();
        await InvokeAsync(StateHasChanged);
    }
}
