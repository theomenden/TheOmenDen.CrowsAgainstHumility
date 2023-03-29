using TheOmenDen.CrowsAgainstHumility.Core.Models.Captchas;

namespace TheOmenDen.CrowsAgainstHumility.Services;

public interface IRecaptchaService
{
    Task<CaptchaResponseDto> VerifyCaptchaAsync(String? token, CancellationToken cancellationToken = default);
}
