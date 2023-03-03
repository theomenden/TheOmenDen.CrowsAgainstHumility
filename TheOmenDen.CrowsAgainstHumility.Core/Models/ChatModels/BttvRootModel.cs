using System.Text.Json.Serialization;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.ChatModels;
public sealed class BttvRootModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = String.Empty;
    
    [JsonPropertyName("bots")]
    public string[] Bots { get; set; } = Array.Empty<string>();

    [JsonPropertyName("avatar")]
    public string Avatar { get; set; } = String.Empty;

    [JsonPropertyName("channelEmotes")]
    public BttvChannelEmote[] ChannelEmotes { get; set; } = Array.Empty<BttvChannelEmote>();

    [JsonPropertyName("sharedEmotes")]
    public BttvSharedEmote[] SharedEmotes { get; set; } = Array.Empty<BttvSharedEmote>();
}
