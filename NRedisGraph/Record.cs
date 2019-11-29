using StackExchange.Redis;

namespace NRedisGraph
{
    public sealed class Record
    {
        public string GetString(int index) => default;
        
        internal static Record Parse(RedisResult result)
        {
            return default;
        }
    }
}