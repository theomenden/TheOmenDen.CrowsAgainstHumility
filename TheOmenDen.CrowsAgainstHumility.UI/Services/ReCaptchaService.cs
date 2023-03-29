using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;
using NuGet.Common;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Captchas;
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

    public async Task<CaptchaResponseDto> VerifyCaptchaAsync(String? token, CancellationToken cancellationToken = default)
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

            return deserializedCaptchaResponse!;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Connection failed captcha: {@Ex}", ex);
            throw;
        }
    }

    private async Task LoadRecaptchaAsync(String captchaUri, CancellationToken cancellationToken = default)
        => await _jsRuntime.InvokeVoidAsync("loadJsById", cancellationToken,"recaptchaScript", captchaUri);
}
