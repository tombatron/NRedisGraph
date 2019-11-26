using StackExchange.Redis;
using System.Collections;
using System.Collections.Generic;

namespace NRedisGraph
{
    public sealed class ResultSet : IReadOnlyCollection<Record>
    {
        public enum ResultSetScalarType
        {
            PROPERTY_UNKNOWN,
            PROPERTY_NULL,
            PROPERTY_STRING,
            PROPERTY_INTEGER,
            PROPERTY_BOOLEAN,
            PROPERTY_DOUBLE
        }

        private readonly RedisResult _result;

        public ResultSet(RedisResult result)
        {
            _result = result;
        }

        public Statistics Statistics { get; }

        public Header Header { get; }

        public int Count =>
        throw new System.NotImplementedException();

        public IEnumerator<Record> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}