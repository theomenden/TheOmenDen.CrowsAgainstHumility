using System.Text.Json.Serialization;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Players;

namespace TheOmenDen.CrowsAgainstHumility.Core.DTO.Serializations;
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Default, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Lobby))]
[JsonSerializable(typeof(LobbyData))]
public partial class LobbySerializerJsonContext : JsonSerializerContext
{
}
