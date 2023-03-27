using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;
using NuGet.Common;
using TheOmenDen.Shared.Guards;

namespace TheOmenDen.CrowsAgainstHumility.Services;

internal sealed class ReCaptchaService: IRecaptchaService
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

        var uri = $"{_settings.CaptchaUri}{_settings.SiteKey}";

        var isCaptchaLoaded = await _jsRuntime.InvokeAsync<bool>("isRecaptchaLoaded", cancellationToken, uri);

        if(!isCaptchaLoaded)
        {
            await LoadRecaptchaAsync(uri, cancellationToken);
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
            using var httpClient = _httpClientFactory.CreateClient("captchaClient");
            
            using var encodedContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new ("secret", _settings.SecretKey),
                new ("response", token)
            });

            using var response = await httpClient.PostAsync(httpClient.BaseAddress, encodedContent, cancellationToken);

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

    private async Task LoadRecaptchaAsync(String captchaUri, CancellationToken cancellationToken = default)
        => await _jsRuntime.InvokeVoidAsync("loadJsById", cancellationToken,"recaptchaScript", captchaUri);
}
