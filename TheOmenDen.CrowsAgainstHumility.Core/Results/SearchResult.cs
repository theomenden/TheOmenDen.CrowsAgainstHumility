
namespace TheOmenDen.CrowsAgainstHumility.Core.Results;

public sealed record SearchResult<T>(Int32 Total, IEnumerable<T> Data);
