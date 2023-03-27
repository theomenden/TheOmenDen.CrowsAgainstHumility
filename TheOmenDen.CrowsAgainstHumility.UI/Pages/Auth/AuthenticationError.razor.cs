using System.Runtime.CompilerServices;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Pages.Auth;

public partial class AuthenticationError : ComponentBase
{
    #region Private Members
    private string _errorText = String.Empty;
    #endregion
    #region Parameters
    public String ErrorEnumValue { get; set; }
    public String Description { get; set; }
    #endregion
    #region Injected Members
    [Inject] private NavigationManager NavigationManager { get; init; }
    #endregion
    #region Lifecycle Methods
    protected override void OnInitialized()
    {
        var (isSuccess, result) = ExternalAuthenticationResult.TryParse(ErrorEnumValue, true);

        if (!isSuccess)
        {
            result = ExternalAuthenticationResult.Unknown;
        }

        result
            .When(ExternalAuthenticationResult.UserCreationFailed)
                .Then(() => _errorText = "User cannot be created")
            .When(ExternalAuthenticationResult.UserIsNotAllowed)
                .Then(() => _errorText = "Login not allowed, check email inbox for account confirmation")
            .When(ExternalAuthenticationResult.UserIsLockedOut)
                .Then(() => _errorText = "User is locked out")
            .When(ExternalAuthenticationResult.CannotAddExternalLogin)
                .Then(() => _errorText = "Cannot create a binding for this external login provider to the current account")
            .When(ExternalAuthenticationResult.ExternalAuthenticationError)
                .Then(() => _errorText = "External provide could not authenticate.\r\nCheck configuration")
            .When(ExternalAuthenticationResult.ExternalUnknownUserId)
                .Then(() => _errorText = "Chosen provider did not pass user identifier")
            .When(ExternalAuthenticationResult.ProviderNotFound)
                .Then(() => _errorText = "Chosen provider has not been configured")
            .When(ExternalAuthenticationResult.DomainError)
                .Then(() => _errorText = String.Empty)
            .When(ExternalAuthenticationResult.Unknown)
                .Then(() => _errorText = "Unknown Reason")
            .DefaultCondition(() => _errorText = "Unknown Reason");
    }
    #endregion
}
