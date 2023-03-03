using System.Text.Json.Serialization;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.ChatModels;

public sealed class BttvSharedEmote: BttvBaseEmote
{
    [JsonPropertyName("user")]
    public BttvUser User { get; set; }
}
