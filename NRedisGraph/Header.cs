using System.Collections.Generic;
using StackExchange.Redis;

namespace NRedisGraph
{
    public sealed class Header
    {
        public enum ResultSetColumnTypes
        {
            COLUMN_UNKNOWN,
            COLUMN_SCALAR,
            COLUMN_NODE,
            COLUMN_RELATION
        }

        public List<ResultSetColumnTypes> SchemaTypes { get; }

        public List<string> SchemaNames { get; }

        internal Header(RedisResult result)
        {
            SchemaTypes = new List<ResultSetColumnTypes>();
            SchemaNames = new List<string>();

            foreach(RedisResult[] tuple in (RedisResult[])result)
            {
                SchemaTypes.Add((ResultSetColumnTypes)(int)tuple[0]);
                SchemaNames.Add((string)tuple[1]);
            }
        }
    }
}