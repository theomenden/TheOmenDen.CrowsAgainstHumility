namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Exceptions;
public sealed class CouldNotChangePlayerTypeException: Exception
{
    public CouldNotChangePlayerTypeException(string message)
    : base(message) 
    {}

    public CouldNotChangePlayerTypeException(string message, Exception innerException)
    : base(message, innerException)
    {
    }
}
