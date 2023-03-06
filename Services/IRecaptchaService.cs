namespace TheOmenDen.CrowsAgainstHumility.Services;

public interface IRecaptchaService
{
    ValueTask<String> GenerateCaptchaTokenAsync(String? action, CancellationToken cancellationToken = default);
    ValueTask<bool> VerifyCaptchaAsync(String? token, CancellationToken cancellationToken = default);
}
