using StackExchange.Redis;
using TheOmenDen.CrowsAgainstHumility.Azure.Messages;

namespace TheOmenDen.CrowsAgainstHumility.Redis.MessageConverters;
public interface IRedisMessageConverter
{
    RedisValue ConvertToRedisMessage(NodeMessage message);
    NodeMessage ConvertToNodeMessage(RedisValue message);
    NodeMessage GetMessageHeader(RedisValue message);
}
