namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Exceptions;
public sealed class CannotClearBoardException : Exception
{
    public CannotClearBoardException() { }
    public CannotClearBoardException(string message) : base(message) { }
    public CannotClearBoardException(string message, Exception innerException) : base(message, innerException) { }
}
