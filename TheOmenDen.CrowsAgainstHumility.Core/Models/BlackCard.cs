using System.Text.Json.Serialization;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class BlackCard
{
    [JsonIgnore]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("text")]
    public String Message { get; set; } = String.Empty;

    [JsonPropertyName("pick")]
    public Int32 PickAnswersCount { get; set; }

    [JsonPropertyName("pack")]
    public Int32 PackId { get; set; }
}