namespace TheOmenDen.CrowsAgainstHumility.Bootstrapping;

public static class ExceptionTemplates
{
    public const string DelegateExceptionTemplate = @"No callback registered for {0}";
    public const string ServerValidationHandlerRequired = @"Validation handler required for server side recaptcha {0}";
    public const string ServerValidationExceptionTemplate = @"Error with server validation callback";
}
