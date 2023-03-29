namespace TheOmenDen.CrowsAgainstHumility.Core.Exceptions;
public sealed class CaptchaLoadingScriptException: Exception
{
    public CaptchaLoadingScriptException() {}
    public CaptchaLoadingScriptException(string message): base(message) {}
    public CaptchaLoadingScriptException(string message, Exception innerException): base(message,innerException) {}
}
