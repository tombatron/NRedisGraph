using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace NRedisGraph
{
    /// <summary>
    /// Query response header interface. Represents the response schema (column names and types).
    /// </summary>
    public sealed class Header
    {
        /// <summary>
        /// The expected column types.
        /// </summary>
        public enum ResultSetColumnTypes
        {
            /// <summary>
            /// Who can say?
            /// </summary>
            COLUMN_UNKNOWN,

            /// <summary>
            /// A single value.
            /// </summary>
            COLUMN_SCALAR,

            /// <summary>
            /// Refers to an actual node.
            /// </summary>
            COLUMN_NODE,

            /// <summary>
            /// Refers to a relation.
            /// </summary>            
            COLUMN_RELATION
        }

        /// <summary>
        /// Collection of the schema types present in the header.
        /// </summary>
        /// <value></value>
        [Obsolete("SchemaType is no longer supported after RedisGraph 2.1 and will always return COLUMN_SCALAR")]
        public List<ResultSetColumnTypes> SchemaTypes { get; }

        /// <summary>
        /// Collection of the schema names present in the header.
        /// </summary>
        /// <value></value>
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