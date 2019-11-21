using System.Collections.Generic;

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
    }
}