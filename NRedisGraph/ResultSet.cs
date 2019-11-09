using System.Collections;
using System.Collections.Generic;

namespace NRedisGraph
{
    public sealed class ResultSet : IEnumerable<Record>
    {
        public Statistics Statistics { get; }

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