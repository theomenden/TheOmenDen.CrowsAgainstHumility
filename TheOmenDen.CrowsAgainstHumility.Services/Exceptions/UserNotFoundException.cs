using System.Runtime.Serialization;

namespace TheOmenDen.CrowsAgainstHumility.Services.Exceptions;
[Serializable]
public sealed class UserNotFoundException : Exception
{
    public UserNotFoundException()
    {
    }

    public UserNotFoundException(string message) : base(message)
    {
    }

    public UserNotFoundException(string message, Exception innerException) :
        base(message, innerException)
    {
    }

    private UserNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
    }
}
