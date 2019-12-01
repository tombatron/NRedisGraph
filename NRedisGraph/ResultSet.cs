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

            if (_result.Length == 3)
            {
                Header = new Header(_result[0]);
                Statistics = new Statistics(_result[2]);
            }
            else
            {
                
                Statistics = new Statistics(_result[_result.Length - 1]);
            }
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