using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IO;
using System.Text;
using System.Text.Json;
using System;

namespace TheOmenDen.CrowsAgainstHumility.Services.Helpers;
public static class CachingHelper
{
    public static async Task SetRecordAsync<T>(this IDistributedCache cache,
        string recordId, T data, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromSeconds(3600),
            SlidingExpiration = slidingExpiration
        };

        var jsonData = JsonSerializer.Serialize(data);
        
        await cache.SetStringAsync(recordId, jsonData, options);
    }

    public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, String recordId)
    {
        var jsonData = await cache.GetStringAsync(recordId);

        return jsonData is null 
            ? default 
            : JsonSerializer.Deserialize<T>(jsonData);
    }
}
