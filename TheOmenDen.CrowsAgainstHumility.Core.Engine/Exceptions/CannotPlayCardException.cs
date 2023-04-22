namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Exceptions;
public sealed class CannotPlayCardException : Exception
{
    public CannotPlayCardException() { }
    public CannotPlayCardException(string message) : base(message) { }
    public CannotPlayCardException(string message, Exception innerException) : base(message, innerException) { }
}
