namespace TheOmenDen.CrowsAgainstHumility.Exceptions;

public sealed class CallbackDelegateException : Exception
{
    public CallbackDelegateException() {}

    public CallbackDelegateException(string message): base(message) {}

    public CallbackDelegateException(string message, Exception innerException): base(message, innerException) {}
}
