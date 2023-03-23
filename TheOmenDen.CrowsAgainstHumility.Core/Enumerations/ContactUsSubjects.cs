using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;
public sealed record ContactUsSubjects : EnumerationBase<ContactUsSubjects>
{
    private ContactUsSubjects(string name, int id) : base(name, id) { }

    public static readonly ContactUsSubjects Support = new(nameof(Support), 1);
    public static readonly ContactUsSubjects General = new(nameof(General), 2);
    public static readonly ContactUsSubjects Unsubscribe = new(nameof(Unsubscribe), 3);
    public static readonly ContactUsSubjects Partner = new(nameof(Partner), 4);
}
