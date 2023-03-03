using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models.ChatModels;

namespace TheOmenDen.CrowsAgainstHumility.Twitch.Services;
internal sealed class BetterTTVEmoteService: IBetterTTVEmoteService
{
    private const string EmoteEndpoint = "cached/emotes/global";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BetterTTVEmoteService> _logger;

    public BetterTTVEmoteService(IHttpClientFactory httpClientFactory, ILogger<BetterTTVEmoteService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async IAsyncEnumerable<BttvBaseEmote> GetBetterTtvGlobalEmotesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to retrieve Bttv Global Emotes");
        using var client = _httpClientFactory.CreateClient("bttvClient");

        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(client.BaseAddress + EmoteEndpoint));

        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        await using var responseContent = await response.Content.ReadAsStreamAsync(cancellationToken);

        await foreach (var globalEmote in JsonSerializer.DeserializeAsyncEnumerable<BttvGlobalEmote>(responseContent, JsonSerializerOptions, cancellationToken))
        {
            yield return globalEmote ?? new ();

            _logger.LogInformation("Emote deserialized: {EmoteName}", globalEmote?.Code ?? String.Empty);
        }
    }
}
