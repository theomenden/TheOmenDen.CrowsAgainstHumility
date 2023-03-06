using System.Text.RegularExpressions;

namespace TheOmenDen.CrowsAgainstHumility.Core.Constants;
public static class RegexPatterns
{

    private const string UserPattern = @"((?<=^)|(?<=\s))@{1}[-_\w]+";
    
    public const string UserName = @"([a-zA-Z0-9_-]+)";

    public static Lazy<Regex> IsUserRegex = new(() => new(UserPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled));
}
