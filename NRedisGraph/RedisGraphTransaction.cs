using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRedisGraph
{
    public class RedisGraphTransaction
    {
        private class TransactionResult
        {
            public string GraphId { get; }

            public Task<RedisResult> PendingTask { get; }

            public TransactionResult(string graphId, Task<RedisResult> pendingTask)
            {
                GraphId = graphId;
                PendingTask = pendingTask;
            }
        }

        private readonly ITransaction _transaction;
        private readonly IDictionary<string, GraphCache> _graphCaches;
        private readonly RedisGraph _redisGraph;
        private readonly List<TransactionResult> _pendingTasks = new List<TransactionResult>();
        private readonly List<string> _graphCachesToRemove = new List<string>();

        internal RedisGraphTransaction(ITransaction transaction, RedisGraph redisGraph, IDictionary<string, GraphCache> graphCaches)
        {
            _graphCaches = graphCaches;
            _redisGraph = redisGraph;
            _transaction = transaction;
        }

        public ValueTask QueryAsync(string graphId, string query, IDictionary<string, object> parameters)
        {
            var preparedQuery = RedisGraph.PrepareQuery(query, parameters);

            return QueryAsync(graphId, preparedQuery);
        }

        public ValueTask QueryAsync(string graphId, string query)
        {
            _graphCaches.PutIfAbsent(graphId, new GraphCache(graphId, _redisGraph));

            _pendingTasks.Add(new TransactionResult(graphId, _transaction.ExecuteAsync(Command.QUERY, graphId, query, RedisGraph.CompactQueryFlag)));

            return default(ValueTask);
        }

        public ValueTask CallProcedureAsync(string graphId, string procedure) =>
            CallProcedureAsync(graphId, procedure, Enumerable.Empty<string>(), RedisGraph.EmptyKwargsDictionary);

        public ValueTask CallProcedureAsync(string graphId, string procedure, IEnumerable<string> args, Dictionary<string, List<string>> kwargs)
        {
            args = args.Select(a => RedisGraph.QuoteString(a));

            var queryBody = new StringBuilder();

            queryBody.Append($"CALL {procedure}({string.Join(",", args)})");

            if (kwargs.TryGetValue("y", out var kwargsList))
            {
                queryBody.Append(string.Join(",", kwargsList));
            }

            return QueryAsync(graphId, queryBody.ToString());
        }

        public ValueTask DeleteGraphAsync(string graphId)
        {
            _pendingTasks.Add(new TransactionResult(graphId, _transaction.ExecuteAsync(Command.DELETE, graphId)));

            _graphCachesToRemove.Add(graphId);

            return default(ValueTask);
        }

        public ResultSet[] Exec()
        {
            var results = new ResultSet[_pendingTasks.Count];

            var success = _transaction.Execute(); // TODO: Handle false (which means the transaction didn't succeed.)

            for (var i = 0; i < _pendingTasks.Count; i++)
            {
                var result = _pendingTasks[i].PendingTask.Result;
                var graphId = _pendingTasks[i].GraphId;

                results[i] = new ResultSet(result, _graphCaches[graphId]);
            }

            ProcessPendingGraphCacheRemovals();

            return results;
        }

        public async Task<ResultSet[]> ExecAsync()
        {
            var results = new ResultSet[_pendingTasks.Count];

            var success = await _transaction.ExecuteAsync();

            for (var i = 0; i < _pendingTasks.Count; i++)
            {
                var result = _pendingTasks[i].PendingTask.Result;
                var graphId = _pendingTasks[i].GraphId;

                results[i] = new ResultSet(result, _graphCaches[graphId]);
            }

            ProcessPendingGraphCacheRemovals();

            return results;
        }

        private void ProcessPendingGraphCacheRemovals()
        {
            foreach(var graph in _graphCachesToRemove)
            {
                _graphCaches.Remove(graph);
            }
        }
    }
}