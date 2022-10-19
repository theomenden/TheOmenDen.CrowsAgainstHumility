using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Criteria;
public sealed class SortCriteria
{
    public SortCriteria(String field, SortType sortType)
    {
        Field = field;
        SortType = sortType;
    }

    public String Field { get; set; }

    public SortType SortType { get; set; }
}
