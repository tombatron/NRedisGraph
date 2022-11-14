using StackExchange.Redis;

namespace NRedisGraph
{
    public interface IRedisGraphFactory
    {
        IRedisGraph Build(IDatabase db);
    }
}
