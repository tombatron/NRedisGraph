using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NRedisGraph
{
    public sealed class GraphCacheList
    {
        private readonly object mutex = new object();
        private readonly string _graphId;
        private readonly string _procedure;
        private readonly RedisGraph _redisGraph;
        private readonly ConcurrentBag<string> _data = new ConcurrentBag<string>();

        public GraphCacheList(string graphId, string procedure, RedisGraph redisGraph)
        {
            _graphId = graphId;
            _procedure = procedure;
            _redisGraph = redisGraph;
        }

        public string GetCachedData(int index)
        {
            if (index >= _data.Count)
            {
                lock(mutex)
                {
                    if (index >= _data.Count)
                    {
                        GetProcedureInfo();
                    }
                }
            }

            return _data.ElementAtOrDefault(index);
        }

        private void GetProcedureInfo()
        {
            var resultSet = _redisGraph.CallProcedure(_graphId, _procedure);
            var newData = new List<string>();
            var i = 0;

            foreach(var record in resultSet)
            {
                if (i >= _data.Count)
                {
                    _data.Add(record.GetString(0));
                }

                i++;
            }
        }
    }
}