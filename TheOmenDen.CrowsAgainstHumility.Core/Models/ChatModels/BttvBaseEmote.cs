using System.Text.Json.Serialization;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.ChatModels;
public abstract class BttvBaseEmote
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = String.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = String.Empty;

    [JsonPropertyName("imageType")] 
    public String ImageType { get; set; } = String.Empty;

    [JsonPropertyName("animated")]
    public Boolean IsAnimated { get; set; }
}
