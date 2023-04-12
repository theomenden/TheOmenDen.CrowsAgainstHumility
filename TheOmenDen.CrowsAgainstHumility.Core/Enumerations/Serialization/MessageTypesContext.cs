using System.Text.Json.Serialization;
using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations.Serialization;
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, GenerationMode = JsonSourceGenerationMode.Default, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(IEnumerationBase))]
[JsonSerializable(typeof(MessageTypes))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
public partial class MessageTypesContext: JsonSerializerContext
{
}
