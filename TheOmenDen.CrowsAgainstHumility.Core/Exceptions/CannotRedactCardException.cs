namespace TheOmenDen.CrowsAgainstHumility.Core.Exceptions;
public sealed class CannotRedactCardException : Exception
{
    public CannotRedactCardException() {}
    public CannotRedactCardException(string message) : base(message){}
    public CannotRedactCardException(string message, Exception innerException) : base(message, innerException) { }
}
