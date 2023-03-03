using System.Text.Json.Serialization;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.ChatModels;
public sealed class BttvUser
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = String.Empty;

    [JsonPropertyName("name")] 
    public String Name { get; set; } = String.Empty;

    [JsonPropertyName("displayName")] 
    public String DisplayName { get; set; } = String.Empty;
    
    [JsonPropertyName("providerId")] 
    public String ProviderId { get; set; } = String.Empty;
}
