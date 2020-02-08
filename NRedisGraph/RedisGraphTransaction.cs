using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRedisGraph
{
    public class RedisGraphTransaction
    {
        private readonly ITransaction _transaction;
        private readonly IDictionary<string, GraphCache> _graphCaches;
        private readonly RedisGraph _redisGraph;
        private readonly List<Task<RedisResult>> _pendingTasks = new List<Task<RedisResult>>();

        public RedisGraphTransaction(ITransaction transaction, RedisGraph redisGraph, IDictionary<string, GraphCache> graphCaches)
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

            _pendingTasks.Add(_transaction.ExecuteAsync(Command.QUERY, graphId, query, RedisGraph.CompactQueryFlag));

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
            _pendingTasks.Add(_transaction.ExecuteAsync(Command.DELETE, graphId));

            _graphCaches.Remove(graphId);

            return default(ValueTask);
        }

        public ResultSet[] Exec()
        {
            var results = new ResultSet[_pendingTasks.Count];

            var success = _transaction.Execute(); // TODO: Handle false (which means the transaction didn't succeed.)

            for(var i = 0; i < _pendingTasks.Count; i++)
            {
                results[i] = new ResultSet(_pendingTasks[i].Result, _graphCaches); // TODO: Store pending task with the name of the graph it was executed against.
            }

            return results;
        }

        public Task<ResultSet[]> ExecAsync()
        {
            return default;
        }
    }
}