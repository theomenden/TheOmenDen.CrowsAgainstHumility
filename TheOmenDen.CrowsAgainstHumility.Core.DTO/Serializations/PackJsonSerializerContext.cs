using System.Text.Json.Serialization;
using TheOmenDen.CrowsAgainstHumility.Core.DAO.Models.Cards;

namespace TheOmenDen.CrowsAgainstHumility.Core.Transformation.Serializations;
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(Pack))]
[JsonSerializable(typeof(WhiteCard))]
[JsonSerializable(typeof(BlackCard))]
public partial class PackJsonSerializerContext: JsonSerializerContext {}
