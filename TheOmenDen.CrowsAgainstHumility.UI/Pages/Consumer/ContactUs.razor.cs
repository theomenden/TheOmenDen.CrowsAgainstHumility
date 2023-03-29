using Blazorise;
using Microsoft.AspNetCore.Identity.UI.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Constants;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Captchas;
using TheOmenDen.CrowsAgainstHumility.Events;
using TheOmenDen.CrowsAgainstHumility.Services;

namespace TheOmenDen.CrowsAgainstHumility.Pages.Consumer;

public partial class ContactUs : ComponentBase
{
    #region Private Members
    private Validations _validations;
    private ContactFormInputModel _formInput = new();
    private ContactUsSubjects _chosenSubject = ContactUsSubjects.General;
    private string _captchaErrors = String.Empty;
    private bool _captchaErrorsVisible = false;
    private bool _isDisabled = true;
    private Alert _captchaAlert = new();
    #endregion
    #region Injected Members
    [Inject] private IRecaptchaService ReCaptchaService { get; init; }
    [Inject] private IEmailSender EmailSender { get; init; }
    #endregion
    #region Private Methods
    private async Task SubmitContactFormAsync()
    {
        if (!(await _validations.ValidateAll()))
        {
            return;
        }
        
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

    private async Task ClearContactFormAsync()
    {
        _formInput = new();
        await _captchaAlert.Hide();
        await _validations.ClearAll();
        await InvokeAsync(StateHasChanged);
    }
    #endregion
    #region Captcha Methods

    private Task TimeoutCallback(CaptchaTimeoutEventArgs e)
    {
        _captchaErrors = $"Captcha timed out: {e.ErrorMessage}";
        return Task.CompletedTask;
    }

    private Task SuccessCallback(CaptchaSuccessEventArgs e)
    {
        _isDisabled = false;
        
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task ValidationErrorCallback(CaptchaValidationErrorEventArgs e)
    {
        _captchaErrors = $"Recaptcha failed validation: {e.ErrorMessage}";
        return Task.CompletedTask;
    }
    private async Task<CaptchaValidationResponseModel> ValidationHandlerCallback(CaptchaValidationRequestModel model)
    {
        var captchaResult = await ReCaptchaService.VerifyCaptchaAsync(model.CaptchaResponse);

        var validationMessage = captchaResult.ErrorCodes.Any()
            ? String.Join(Environment.NewLine, captchaResult.ErrorCodes)
            : "No Errors";

        return new CaptchaValidationResponseModel(captchaResult.Success, validationMessage);
    }
    #endregion
}
