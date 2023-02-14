using TheOmenDen.Shared.Enumerations;
using TheOmenDen.Shared.Enumerations.Attributes;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

[AllowNegativeEnumerationKeys]
public sealed record ExternalAuthenticationResult: EnumerationBase<ExternalAuthenticationResult>
{
    private ExternalAuthenticationResult(string name, int id) 
        : base(name, id)
    {
    }

    public static readonly ExternalAuthenticationResult Unknown = new (nameof(Unknown),-99);
    public static readonly ExternalAuthenticationResult UserCreationFailed = new (nameof(UserCreationFailed),-1);
    public static readonly ExternalAuthenticationResult UserIsNotAllowed = new (nameof(UserIsNotAllowed),0);
    public static readonly ExternalAuthenticationResult UserIsLockedOut = new (nameof(UserIsLockedOut),1);
    public static readonly ExternalAuthenticationResult CannotAddExternalLogin = new (nameof(CannotAddExternalLogin),2);
    public static readonly ExternalAuthenticationResult ExternalAuthenticationError = new (nameof(ExternalAuthenticationError),3);
    public static readonly ExternalAuthenticationResult ExternalUnknownUserId = new (nameof(ExternalUnknownUserId),4);
    public static readonly ExternalAuthenticationResult ProviderNotFound = new (nameof(ProviderNotFound),5);
    public static readonly ExternalAuthenticationResult DomainError = new (nameof(DomainError),6);
}
