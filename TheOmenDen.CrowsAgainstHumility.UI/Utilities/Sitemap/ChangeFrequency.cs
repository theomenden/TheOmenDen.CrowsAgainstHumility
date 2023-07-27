using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Utilities.Sitemap;

public sealed record ChangeFrequency : EnumerationBase<ChangeFrequency>
{
    private ChangeFrequency(String name, Int32 value) : base(name, value) { }

    public static readonly ChangeFrequency Always = new("Always", 1);
    public static readonly ChangeFrequency Hourly = new("Hourly", 2);
    public static readonly ChangeFrequency Daily = new("Daily", 3);
    public static readonly ChangeFrequency Weekly = new("Weekly", 4);
    public static readonly ChangeFrequency Monthly = new("Monthly", 5);
    public static readonly ChangeFrequency Yearly = new("Yearly", 6);
    public static readonly ChangeFrequency Never = new("Never", 7);
}
