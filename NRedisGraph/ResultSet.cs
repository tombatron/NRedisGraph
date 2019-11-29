using System.Collections;
using System.Collections.Generic;
using StackExchange.Redis;

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

        private readonly RedisResult[] _result;
        private readonly GraphCache _graphCache;

        public ResultSet(RedisResult result, GraphCache graphCache)
        {
            _result = (RedisResult[])result;
            _graphCache = graphCache;

            Statistics = new Statistics(_result[0]);
            Header = Header.Parse(_result);
        }

        public Statistics Statistics { get; }

        public Header Header { get; }

        public int Count => 0;

        public IEnumerator<Record> GetEnumerator() => RecordIterator().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => RecordIterator().GetEnumerator();

        private IEnumerable<Record> RecordIterator()
        {
            yield break;
        }
    }
}