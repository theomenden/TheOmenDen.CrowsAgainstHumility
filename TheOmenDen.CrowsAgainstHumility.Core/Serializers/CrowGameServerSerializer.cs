using System.Text.Json;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Core.Serializers;
public class CrowGameServerSerializer
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new();
    private readonly DateTimeProvider _dateTimeProvider;
    private readonly GuidProvider _guidProvider;

    static CrowGameServerSerializer()
    {
        _jsonSerializerOptions.AddContext<CrowGameServerSerializerContext>();
    }

    public CrowGameServerSerializer(DateTimeProvider? dateTimeProvider, GuidProvider? guidProvider)
    {
        _dateTimeProvider = dateTimeProvider ?? DateTimeProvider.Default;
        _guidProvider = guidProvider ?? GuidProvider.Default;
    }

    public void SerializeIntoStream(Stream utf8Json, CrowGameServer lobby)
    {
        ArgumentNullException.ThrowIfNull(utf8Json);
        ArgumentNullException.ThrowIfNull(lobby);

        var data = JsonSerializer.SerializeToUtf8Bytes(lobby, JsonSerializerOptions.Default);
        
        utf8Json.Write(data.AsSpan());
    }

    public async Task SerializeIntoStreamAsync(Stream utf8Json, CrowGameServer lobby,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(utf8Json);
        ArgumentNullException.ThrowIfNull(lobby);

        var data = JsonSerializer.SerializeToUtf8Bytes(lobby);

        await utf8Json.WriteAsync(data, cancellationToken);
        await utf8Json.FlushAsync(cancellationToken);
    }
    public async ValueTask<CrowGameServer> DeserializeFromStreamAsync(Stream utf8Json, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(utf8Json);

        var data = await JsonSerializer.DeserializeAsync<CrowGameServer>(utf8Json, _jsonSerializerOptions, cancellationToken);

        return data ?? throw new InvalidOperationException(Resources.Resources.DeserializationFailed);
    }
    
    public CrowGameServer DeserializeFromStream(Stream utf8Json)
    {
        ArgumentNullException.ThrowIfNull(utf8Json);

        var data = JsonSerializer.Deserialize<CrowGameServer>(utf8Json, _jsonSerializerOptions);

        return data ?? throw new InvalidOperationException(Resources.Resources.DeserializationFailed);
    }
}
