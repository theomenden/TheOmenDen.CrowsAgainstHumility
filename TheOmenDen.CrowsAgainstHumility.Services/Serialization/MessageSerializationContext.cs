using System.Text.Json.Serialization;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
using TheOmenDen.CrowsAgainstHumility.Services.Messages;

namespace TheOmenDen.CrowsAgainstHumility.Services.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = true)]
[JsonSerializable(typeof(Message))]
[JsonSerializable(typeof(MessageTypes))]
internal partial class MessageSerializationContext: JsonSerializerContext
{
}
