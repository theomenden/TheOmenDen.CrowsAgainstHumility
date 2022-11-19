using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record NotificationType: EnumerationBase<NotificationType>
{
    private NotificationType(string name, int id) : base(name, id)
    {
    }

    public static readonly NotificationType Participating = new(nameof(Participating), 0);
    
    public static readonly NotificationType All = new(nameof(All), 1);
    
    public static readonly NotificationType Ignore = new(nameof(Ignore), 2);
}
