using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

public sealed record SortType: EnumerationBase<SortType>
{
    private SortType(string name, int id) : base(name, id)
    {
    }

    public static readonly SortType None = new(nameof(None), 1);

    public static readonly SortType Ascending = new(nameof(Ascending), 2);

    public static readonly SortType Descending = new(nameof(Descending), 3);
}
