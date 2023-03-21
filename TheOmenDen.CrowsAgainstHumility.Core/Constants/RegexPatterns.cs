using System.Text.RegularExpressions;

namespace TheOmenDen.CrowsAgainstHumility.Core.Constants;
public static class RegexPatterns
{
    #region Constants
    public const string UserPattern = @"((?<=^)|(?<=\s))@{1}[-_\w]+";
    public const string Username = @"([a-zA-Z0-9_-]+)";
    public const string CapitalCheck = @"[A-Z]+";
    public const string LowerCheck = @"[a-z]+";
    public const string NumericCheck = @"[0-9]+";
    public const string SpecialCheck = @"[\!\?\*\.]+";
    #endregion
    #region Lazy Loaded Regex
    public static Lazy<Regex> IsUserRegex = new(() => new(UserPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled));
    public static Lazy<Regex> IsUsernameRegex = new(() => new(Username, RegexOptions.IgnoreCase | RegexOptions.Compiled));
    public static Lazy<Regex> IsCapitalLetterRegex = new(() => new(CapitalCheck, RegexOptions.IgnoreCase | RegexOptions.Compiled));
    public static Lazy<Regex> IsLowerLetterRegex = new(() => new(LowerCheck, RegexOptions.IgnoreCase | RegexOptions.Compiled));
    public static Lazy<Regex> IsNumericRegex = new(() => new(NumericCheck, RegexOptions.IgnoreCase | RegexOptions.Compiled));
    public static Lazy<Regex> IsSpecialRegex = new(() => new(SpecialCheck, RegexOptions.IgnoreCase | RegexOptions.Compiled));
    #endregion
}
