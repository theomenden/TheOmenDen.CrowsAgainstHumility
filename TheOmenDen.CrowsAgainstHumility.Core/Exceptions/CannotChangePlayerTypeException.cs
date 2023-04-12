namespace TheOmenDen.CrowsAgainstHumility.Core.Exceptions;
public sealed class CannotChangePlayerTypeException : Exception
{
    public CannotChangePlayerTypeException() { }
    public CannotChangePlayerTypeException(string message) : base(message) { }
    public CannotChangePlayerTypeException(string message, Exception innerException) : base(message, innerException) { }
}
