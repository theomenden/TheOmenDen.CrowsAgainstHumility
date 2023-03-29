namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Captchas;

public sealed record CaptchaValidationResponseModel(bool IsSuccess = false,
    string ValidationMessage = "Validation handler not registered");
