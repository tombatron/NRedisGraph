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

        public Task QueryAsync(string graphId, string query, IDictionary<string, object> parameters)
        {
            var preparedQuery = RedisGraph.PrepareQuery(query, parameters);

            return QueryAsync(graphId, preparedQuery);
        }

        public Task QueryAsync(string graphId, string query)
        {
            _graphCaches.PutIfAbsent(graphId, new GraphCache(graphId, _redisGraph));

            _pendingTasks.Add(_transaction.ExecuteAsync(Command.QUERY, graphId, query, RedisGraph.CompactQueryFlag));

            return Task.CompletedTask;
        }

        public Task CallProcedureAsync(string graphId, string procedure) =>
            CallProcedureAsync(graphId, procedure, Enumerable.Empty<string>(), RedisGraph.EmptyKwargsDictionary);

        public Task CallProcedureAsync(string graphId, string procedure, IEnumerable<string> args, Dictionary<string, List<string>> kwargs)
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

        public Task DeleteGraphAsync(string graphId)
        {
            return default;
        }

        public IEnumerable<ResultSet> Exec()
        {
            return default;
        }

        public Task<IEnumerable<ResultSet>> ExecAsync()
        {
            return default;
        }
    }
}