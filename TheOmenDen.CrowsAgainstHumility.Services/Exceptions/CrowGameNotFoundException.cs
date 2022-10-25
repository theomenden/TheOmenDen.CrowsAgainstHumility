namespace TheOmenDen.CrowsAgainstHumility.Services.Exceptions;

[Serializable]
public sealed class CrowGameNotFoundException : Exception
{
    public CrowGameNotFoundException()
    {
    }

    public CrowGameNotFoundException(string message) : base(message)
    {
    }

    public CrowGameNotFoundException(string message, Exception innerException) :
        base(message, innerException)
    {
    }

    private CrowGameNotFoundException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
    : base(serializationInfo, streamingContext)
    {
    }
}
