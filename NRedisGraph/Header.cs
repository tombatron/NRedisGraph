using System.Collections.Generic;
using StackExchange.Redis;

namespace NRedisGraph
{
    public class Header
    {
        public enum ResultSetColumnTypes
        {
            COLUMN_UNKNOWN,
            COLUMN_SCALAR,
            COLUMN_NODE,
            COLUMN_RELATION
        }

        public List<string> SchemaNames { get; set; }

        public List<ResultSetColumnTypes> SchemaTypes { get; set; }

        public static Header Parse(RedisResult[] results)
        {
            return default;
        }
    }
}