using System.Text.Json.Serialization;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Captchas;

public sealed class CaptchaResponseDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("error-codes")]
    public IEnumerable<String> ErrorCodes { get; set; } = Enumerable.Empty<string>();
}