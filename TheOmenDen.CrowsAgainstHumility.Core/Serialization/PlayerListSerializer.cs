using System.Text.Json;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Core.Serialization;
public class PlayerListSerializer
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new();
    private readonly DateTimeProvider _dateTimeProvider;
    private readonly GuidProvider _guidProvider;

    static PlayerListSerializer()
    {
        _jsonSerializerOptions.Converters.Add(new CrowGameJsonConverter());
    }

    public PlayerListSerializer(DateTimeProvider? dateTimeProvider, GuidProvider? guidProvider)
    {
        _dateTimeProvider = dateTimeProvider ?? DateTimeProvider.Default;
        _guidProvider = guidProvider ?? GuidProvider.Default;
    }

    public void Serialize(Stream utf8Json, PlayerList playerList)
    {
        if (utf8Json is null)
        {
            throw new ArgumentNullException(nameof(utf8Json));
        }

        if (playerList is null)
        {
            throw new ArgumentNullException(nameof(playerList));
        }

        var data = playerList.GetData();
        JsonSerializer.Serialize(utf8Json, data, _jsonSerializerOptions);
    }

    public PlayerList Deserialize(Stream utf8Json)
    {
        if (utf8Json is null)
        {
            throw new ArgumentNullException(nameof(utf8Json));
        }

        var data = JsonSerializer.Deserialize<PlayerListData>(utf8Json, _jsonSerializerOptions);

        return data is not null
            ?  new PLayerList(data, _dateTimeProvider, _guidProvider)
            : throw new InvalidOperationException();
    }
}
