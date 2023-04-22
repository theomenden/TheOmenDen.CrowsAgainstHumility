namespace TheOmenDen.CrowsAgainstHumility.Core.Engine.Exceptions;
public sealed class MissingPlayerNameException: Exception
{
    public MissingPlayerNameException(): base("Player must have a value for their name!")
    {}
}
