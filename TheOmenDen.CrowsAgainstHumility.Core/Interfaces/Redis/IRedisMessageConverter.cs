using StackExchange.Redis;
using TheOmenDen.CrowsAgainstHumility.Core.Models.Azure;

namespace TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Redis;
public interface IRedisMessageConverter
{
    RedisValue ConvertToRedisMessage(NodeMessage message);
    NodeMessage ConvertToNodeMessage(RedisValue message);
    NodeMessage GetMessageHeader(RedisValue message);
}
