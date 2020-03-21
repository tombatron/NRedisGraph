using System.Linq;

namespace NRedisGraph
{
    internal sealed class GraphCacheList
    {
        private readonly object _locker = new object();
        private readonly string _graphId;
        private readonly string _procedure;
        private readonly RedisGraph _redisGraph;
        private string[] _data;

        internal GraphCacheList(string graphId, string procedure, RedisGraph redisGraph)
        {
            _graphId = graphId;
            _procedure = procedure;
            _redisGraph = redisGraph;
        }

        // TODO: Change this to use Lazy<T>?
        internal string GetCachedData(int index)
        {
            if (_data == null || index >= _data.Length)
            {
                lock(_locker)
                {
                    if (_data == null || index >= _data.Length)
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
            var newData = new string[resultSet.Count];
            var i = 0;

            foreach (var record in resultSet)
            {
                newData[i++] = record.GetString(0);
            }

            _data = newData;
        }
    }
}