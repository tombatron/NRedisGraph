using StackExchange.Redis;

namespace NRedisGraph
{
    public sealed class RedisGraphFactory : IRedisGraphFactory
    {
        public IRedisGraph Build(IDatabase db)
        {
            return new RedisGraph(db);
        }
    }
}
