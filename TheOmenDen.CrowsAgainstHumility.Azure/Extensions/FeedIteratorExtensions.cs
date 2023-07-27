using Microsoft.Azure.Cosmos;

namespace TheOmenDen.CrowsAgainstHumility.Azure.Extensions;
public static class FeedIteratorExtensions
{
    public static async IAsyncEnumerable<TModel> ToAsyncEnumerable<TModel>(this FeedIterator<TModel> setIterator)
    {
        while (setIterator.HasMoreResults)
        {
            foreach (var item in await setIterator.ReadNextAsync().ConfigureAwait(false))
            {
                yield return item;
            }
        }
    }
}
