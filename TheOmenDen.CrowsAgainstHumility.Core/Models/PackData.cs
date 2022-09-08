using System.Text.Json.Serialization;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public class PackData
{
    [JsonIgnore] public Guid Id { get; set; } = Guid.NewGuid();

    public String Name { get; set; } = String.Empty;

    [JsonPropertyName("white")]
    public virtual WhiteCard[] WhiteCards { get; set; } = Array.Empty<WhiteCard>();

    [JsonPropertyName("black")]
    public virtual BlackCard[] BlackCards { get; set; } = Array.Empty<BlackCard>();

    [JsonPropertyName("official")]
    public Boolean IsOfficialPack { get; set; }
}
