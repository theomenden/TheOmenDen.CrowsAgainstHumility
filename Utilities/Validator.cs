using Blazorise;

namespace TheOmenDen.CrowsAgainstHumility.Utilities;

public static class Validator
{
    public static void CompanyValidator(ValidatorEventArgs args)
    => args.Status = Int32.TryParse(args.Value?.ToString(), out var id)
                     && id > 0
            ? ValidationStatus.Success
            : ValidationStatus.Error;
    public static void DateTuneValidator(ValidatorEventArgs args)
        => args.Status = DateTime.TryParse(args.Value?.ToString(), out _)
            ? ValidationStatus.Success
            : ValidationStatus.Error;

    public static void UserValidator(ValidatorEventArgs args)
        => args.Status = !String.IsNullOrWhiteSpace(args.Value?.ToString())
            ? ValidationStatus.Success
            : ValidationStatus.Error;

    public static void GuidNotEmptyValidator(ValidatorEventArgs args)
        => args.Status = Guid.TryParse(args.Value?.ToString(), out var id)
                         && id != Guid.Empty
            ? ValidationStatus.Success
            : ValidationStatus.Error;

    public static void LicenseTypeValidator(ValidatorEventArgs args)
        => args.Status = !String.IsNullOrWhiteSpace(args.Value?.ToString())
            ? ValidationStatus.Success
            : ValidationStatus.Error;
}
