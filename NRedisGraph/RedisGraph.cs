// .NET port of https://github.com/RedisGraph/JRedisGraph
using StackExchange.Redis;

namespace NRedisGraph
{
    public sealed class RedisGraph
    {
        private readonly IDatabase _db;

        public RedisGraph(IDatabase db) => _db = db;

        public ResultSet Query(string graphId, string query, params object[] args)
        {
            return default;
        }
    }
}