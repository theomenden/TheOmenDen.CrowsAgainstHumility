using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Criteria;

public class SearchBase
{
    private List<SortCriteria>? _sortingRules;

    public bool Count { get; set; } = true;

    public int? Offset { get; set; }

    public int? Limit { get; set; }

    public string Value { get; set; }

    public IEnumerable<SortCriteria>  SortingRules => _sortingRules ?? Enumerable.Empty<SortCriteria>();

    public void AddSortCriteria(String field, SortType sortType)
    {
        _sortingRules ??= new();

        _sortingRules.Add(new(field, sortType));
    }
}