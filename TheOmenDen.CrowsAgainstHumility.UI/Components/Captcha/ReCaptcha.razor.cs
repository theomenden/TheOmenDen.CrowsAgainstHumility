using System.ComponentModel;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using TheOmenDen.CrowsAgainstHumility.Bootstrapping;
using TheOmenDen.CrowsAgainstHumility.Core.Exceptions;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Captchas;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Settings;
using TheOmenDen.CrowsAgainstHumility.Events;
using TheOmenDen.CrowsAgainstHumility.Exceptions;

namespace TheOmenDen.CrowsAgainstHumility.Components.Captcha;

public partial class ReCaptcha : ComponentBase
{
    #region Injected Members
    [Inject] private IJSRuntime JsRunTime { get; init; }
    [Inject] internal IOptions<ReCaptchaSettings> CaptchaSettings { get; init; }
    #endregion
    #region Event Callbacks
    [Parameter, EditorRequired]
    public EventCallback<CaptchaSuccessEventArgs> OnSuccess { get; set; }
    [Parameter, EditorRequired]
    public EventCallback<CaptchaTimeoutEventArgs> OnTimeout { get; set; }
    [Parameter]
    public EventCallback<CaptchaValidationErrorEventArgs> OnValidationError { get; set; }
    [Parameter]
    public Func<CaptchaValidationRequestModel, Task<CaptchaValidationResponseModel>> OnValidationHandler { get; set; }
    #endregion
    private ReCaptchaSettings CurrentConfiguration => CaptchaSettings.Value;
    #region LifeCycle Methods
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                var fullCaptchaUri = CaptchaSettings.Value.CaptchaUri + $"?render={CaptchaSettings.Value.SiteKey}";
                await JsRunTime.InvokeVoidAsync("loadJs", fullCaptchaUri);
                await JsRunTime.InvokeVoidAsync("renderRecaptcha", DotNetObjectReference.Create(this),
                    CaptchaSettings.Value.SiteKey);
            }
            catch (Exception ex)
            {
                throw new CaptchaLoadingScriptException("Invalid site key provided, or wrong recaptcha endpoint called.", ex);
            }
        }

        await base.OnInitializedAsync();
    }
    #endregion
    #region JS Invocable Virtual Methods
    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task OnCallbackError(string message)
    {
        if (!OnValidationError.HasDelegate)
        {
            throw new CallbackDelegateException(String.Format(ExceptionTemplates.DelegateExceptionTemplate, nameof(OnValidationError)));
        }

        await InvokeAsync(StateHasChanged);

        await OnValidationError.InvokeAsync(new CaptchaValidationErrorEventArgs(message));
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task OnExpired()
    {
        if (!OnTimeout.HasDelegate)
        {
            throw new CallbackDelegateException(String.Format(ExceptionTemplates.DelegateExceptionTemplate, nameof(OnTimeout)));
        }
        await InvokeAsync(StateHasChanged);
        await OnTimeout.InvokeAsync(new CaptchaTimeoutEventArgs("Recaptcha validation expired"));
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task OnSuccessHandler(string response)
    {
        if (OnValidationHandler is null)
        {
            throw new CallbackDelegateException(String.Format(ExceptionTemplates.DelegateExceptionTemplate,
                nameof(OnValidationHandler)));
        }

        try
        {
            var validationResult = await OnValidationHandler(new CaptchaValidationRequestModel(response));
            if (!validationResult.IsSuccess)
            {
                if (!OnValidationError.HasDelegate)
                {
                    throw new CallbackDelegateException(String.Format(ExceptionTemplates.DelegateExceptionTemplate, nameof(OnValidationError)));
                }

                await OnValidationError.InvokeAsync(new CaptchaValidationErrorEventArgs(validationResult.ValidationMessage));
            }
        }
        catch (Exception ex)
        {
            throw new CallbackDelegateException(ExceptionTemplates.ServerValidationExceptionTemplate,ex);
        }

        if (!OnSuccess.HasDelegate)
        {
            var delegateException = String.Format(ExceptionTemplates.DelegateExceptionTemplate, nameof(OnSuccess));
            throw new CallbackDelegateException(delegateException);
        }

        await OnSuccess.InvokeAsync(new CaptchaSuccessEventArgs(response));
    }
    #endregion
}
