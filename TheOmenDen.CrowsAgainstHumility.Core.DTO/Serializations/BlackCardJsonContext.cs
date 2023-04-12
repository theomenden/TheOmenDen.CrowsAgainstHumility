using System.Text.Json.Serialization;
using TheOmenDen.CrowsAgainstHumility.Core.DTO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Transformation.Serializations;
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Serialization, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(BlackCardDto))]
public partial class BlackCardJsonContext: JsonSerializerContext {}
