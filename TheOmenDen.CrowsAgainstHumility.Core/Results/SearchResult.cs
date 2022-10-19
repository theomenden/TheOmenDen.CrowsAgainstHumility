
namespace TheOmenDen.CrowsAgainstHumility.Core.Results;
public sealed class SearchResult<T>
{
    public SearchResult(Int32 total, IEnumerable<T> data)
    {
    }

    public Int32 Total { get; set; }

    public IEnumerable<T> Data { get; set; }
}
