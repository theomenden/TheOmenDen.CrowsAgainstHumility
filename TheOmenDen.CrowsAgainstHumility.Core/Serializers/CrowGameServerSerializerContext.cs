using System.Text.Json.Serialization;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Game;
using TheOmenDen.CrowsAgainstHumility.Core.Engine.Models;

namespace TheOmenDen.CrowsAgainstHumility.Core.Serializers;
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, 
    WriteIndented = true,
    GenerationMode = JsonSourceGenerationMode.Default,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Player))]
[JsonSerializable(typeof(WhiteCard))]
[JsonSerializable(typeof(BlackCard))]
[JsonSerializable(typeof(GameRoles))]
[JsonSerializable(typeof(IDictionary<string, Player>))]
[JsonSerializable(typeof(CrowGameSession))]
[JsonSerializable(typeof(CrowGameServer))]
public partial class CrowGameServerSerializerContext: JsonSerializerContext
{
}
