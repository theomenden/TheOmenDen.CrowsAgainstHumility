using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;
using TheOmenDen.Shared.Guards;

namespace TheOmenDen.CrowsAgainstHumility.Services;

internal sealed class ReCaptchaService
{
    private readonly ReCaptchaSettings _settings;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<ReCaptchaService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public ReCaptchaService(ReCaptchaSettings settings, IJSRuntime jsRuntime, ILogger<ReCaptchaService> logger, IHttpClientFactory httpClientFactory)
    {
        _settings = settings;
        _jsRuntime = jsRuntime;
        _logger = logger;
        _httpClientFactory = httpClientFactory; 
    }

    public async ValueTask<String> GenerateCaptchaTokenAsync(String? action,
        CancellationToken cancellationToken = default)
    {
        Guard.FromNullOrWhitespace(action, nameof(action));

        var isCaptchaLoaded = await _jsRuntime.InvokeAsync<bool>("isRecaptchaLoaded", cancellationToken, _settings.SiteKey);

        if (!isCaptchaLoaded)
        {
            await LoadRecaptchaAsync(cancellationToken);
        }

        var captchaToken = await _jsRuntime.InvokeAsync<String>("generateCaptchaToken", cancellationToken, _settings.SiteKey,
            action);

        return captchaToken;
    }

    public async ValueTask<bool> VerifyCaptchaAsync(String? token, CancellationToken cancellationToken = default)
    {
        Guard.FromNullOrWhitespace(nameof(token), token);

        try
        {
            using var httpClient = _httpClientFactory.CreateClient();

            var uri = $"{httpClient.BaseAddress}?secret={_settings.SecretKey}&response={token}";

            using var request = new HttpRequestMessage(HttpMethod.Get, uri);

            using var response = await httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var responseContent = await response.Content.ReadAsStreamAsync(cancellationToken);

            var deserializedCaptchaResponse = await
                JsonSerializer.DeserializeAsync<CaptchaResponseDto>(responseContent, SerializerOptions,
                    cancellationToken);

            return deserializedCaptchaResponse is { Success: true, Score: >= 0.5 };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Connection failed captcha: {@Ex}", ex);
            return false;
        }
    }

    private async Task LoadRecaptchaAsync(CancellationToken cancellationToken = default)
        => await _jsRuntime.InvokeVoidAsync("loadRecaptcha", cancellationToken, _settings.SiteKey);
}
