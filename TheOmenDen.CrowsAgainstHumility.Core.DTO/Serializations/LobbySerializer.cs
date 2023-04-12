using System.Text.Json;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Players;
using TheOmenDen.CrowsAgainstHumility.Core.Providers;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Serializations;
public class LobbySerializer
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly DateTimeProvider _dateTimeProvider;
    private readonly GuidProvider _guidProvider;

    public LobbySerializer(DateTimeProvider? dateTimeProvider, GuidProvider? guidProvider)
    {
        _dateTimeProvider = dateTimeProvider ?? DateTimeProvider.Default;
        _guidProvider = guidProvider ?? GuidProvider.Default;
    }

    public void Serialize(Stream utf8Json, Lobby lobby)
    {
        ArgumentNullException.ThrowIfNull(utf8Json);
        ArgumentNullException.ThrowIfNull(lobby);

        var data = lobby.GetData();
        JsonSerializer.Serialize(utf8Json, data, JsonSerializerOptions);
    }

    public Lobby Deserialize(Stream utf8Json)
    {
        ArgumentNullException.ThrowIfNull(utf8Json);

        var data = JsonSerializer.Deserialize<LobbyData>(utf8Json, JsonSerializerOptions);

        ArgumentNullException.ThrowIfNull(data);

        return new Lobby(data, _dateTimeProvider, _guidProvider);
    }
}
