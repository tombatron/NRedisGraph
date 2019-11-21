using System.Collections;
using System.Collections.Generic;

namespace NRedisGraph
{
    public sealed class ResultSet : IReadOnlyCollection<Record>
    {
        public Statistics Statistics { get; }

        public int Count => throw new System.NotImplementedException();

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