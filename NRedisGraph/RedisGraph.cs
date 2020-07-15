// .NET port of https://github.com/RedisGraph/JRedisGraph
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace NRedisGraph
{
    /// <summary>
    /// RedisGraph client.
    /// 
    /// This class facilitates querying RedisGraph and parsing the results.
    /// </summary>
    public sealed class RedisGraph
    {
        internal static readonly object CompactQueryFlag = "--COMPACT";
        private readonly IDatabase _db;
        private readonly IDictionary<string, GraphCache> _graphCaches = new Dictionary<string, GraphCache>();

        private GraphCache GetGraphCache(string graphId)
        {
            if (!_graphCaches.ContainsKey(graphId))
            {
                _graphCaches.Add(graphId, new GraphCache(graphId, this));
            }

            return _graphCaches[graphId];
        }

        /// <summary>
        /// Creates a RedisGraph client that leverages a specified instance of `IDatabase`.
        /// </summary>
        /// <param name="db"></param>
        public RedisGraph(IDatabase db) => _db = db;

        /// <summary>
        /// Execute a Cypher query with parameters.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="parameters">Parameters map.</param>
        /// <returns>A result set.</returns>
        public ResultSet Query(string graphId, string query, IDictionary<string, object> parameters)
        {
            var preparedQuery = PrepareQuery(query, parameters);

            return Query(graphId, preparedQuery);
        }

        /// <summary>
        /// Execute a Cypher query.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <returns>A result set.</returns>
        public ResultSet Query(string graphId, string query)
        {
            _graphCaches.PutIfAbsent(graphId, new GraphCache(graphId, this));

            return new ResultSet(_db.Execute(Command.QUERY, graphId, query, CompactQueryFlag), _graphCaches[graphId]);
        }

        /// <summary>
        /// Execute a Cypher query with parameters.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="parameters">Parameters map.</param>
        /// <returns>A result set.</returns>
        public Task<ResultSet> QueryAsync(string graphId, string query, IDictionary<string, object> parameters)
        {
            var preparedQuery = PrepareQuery(query, parameters);

            return QueryAsync(graphId, preparedQuery);
        }

        /// <summary>
        /// Execute a Cypher query.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <returns>A result set.</returns>
        public async Task<ResultSet> QueryAsync(string graphId, string query)
        {
            _graphCaches.PutIfAbsent(graphId, new GraphCache(graphId, this));

            return new ResultSet(await _db.ExecuteAsync(Command.QUERY, graphId, query, CompactQueryFlag), _graphCaches[graphId]);
        }

        internal static readonly Dictionary<string, List<string>> EmptyKwargsDictionary =
            new Dictionary<string, List<string>>();

        /// <summary>
        /// Call a saved procedure.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <returns>A result set.</returns>
        public ResultSet CallProcedure(string graphId, string procedure) =>
            CallProcedure(graphId, procedure, Enumerable.Empty<string>(), EmptyKwargsDictionary);

        /// <summary>
        /// Call a saved procedure.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <returns>A result set.</returns>
        public Task<ResultSet> CallProcedureAsync(string graphId, string procedure) =>
            CallProcedureAsync(graphId, procedure, Enumerable.Empty<string>(), EmptyKwargsDictionary);

        /// <summary>
        /// Call a saved procedure with parameters.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <returns>A result set.</returns>
        public ResultSet CallProcedure(string graphId, string procedure, IEnumerable<string> args) =>
            CallProcedure(graphId, procedure, args, EmptyKwargsDictionary);

        /// <summary>
        /// Call a saved procedure with parameters.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <returns>A result set.</returns>
        public Task<ResultSet> CallProcedureAsync(string graphId, string procedure, IEnumerable<string> args) =>
            CallProcedureAsync(graphId, procedure, args, EmptyKwargsDictionary);

        /// <summary>
        /// Call a saved procedure with parameters.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="kwargs">A collection of keyword arguments.</param>
        /// <returns>A result set.</returns>
        public ResultSet CallProcedure(string graphId, string procedure, IEnumerable<string> args, Dictionary<string, List<string>> kwargs)
        {
            args = args.Select(a => QuoteString(a));

            var queryBody = new StringBuilder();

            queryBody.Append($"CALL {procedure}({string.Join(",", args)})");

            if (kwargs.TryGetValue("y", out var kwargsList))
            {
                queryBody.Append(string.Join(",", kwargsList));
            }

            return Query(graphId, queryBody.ToString());
        }

        /// <summary>
        /// Call a saved procedure with parameters.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="kwargs">A collection of keyword arguments.</param>
        /// <returns>A result set.</returns>
        public Task<ResultSet> CallProcedureAsync(string graphId, string procedure, IEnumerable<string> args, Dictionary<string, List<string>> kwargs)
        {
            args = args.Select(a => QuoteString(a));

            var queryBody = new StringBuilder();

            queryBody.Append($"CALL {procedure}({string.Join(",", args)})");

            if (kwargs.TryGetValue("y", out var kwargsList))
            {
                queryBody.Append(string.Join(",", kwargsList));
            }

            return QueryAsync(graphId, queryBody.ToString());
        }

        /// <summary>
        /// Create a RedisGraph transaction.
        /// 
        /// This leverages the "Transaction" support present in StackExchange.Redis.
        /// </summary>
        /// <returns></returns>
        public RedisGraphTransaction Multi() =>
            new RedisGraphTransaction(_db.CreateTransaction(), this, _graphCaches);

        /// <summary>
        /// Delete an existing graph.
        /// </summary>
        /// <param name="graphId">The graph to delete.</param>
        /// <returns>A result set.</returns>
        public ResultSet DeleteGraph(string graphId)
        {
            var result = _db.Execute(Command.DELETE, graphId);

            var processedResult = new ResultSet(result, _graphCaches[graphId]);

            _graphCaches.Remove(graphId);

            return processedResult;
        }

        /// <summary>
        /// Delete an existing graph.
        /// </summary>
        /// <param name="graphId">The graph to delete.</param>
        /// <returns>A result set.</returns>
        public async Task<ResultSet> DeleteGraphAsync(string graphId)
        {
            var result = await _db.ExecuteAsync(Command.DELETE, graphId);

            var processedResult = new ResultSet(result, _graphCaches[graphId]);

            _graphCaches.Remove(graphId);

            return processedResult;
        }

        internal static string PrepareQuery(string query, IDictionary<string, object> parms)
        {
            var preparedQuery = new StringBuilder();

            preparedQuery.Append("CYPHER ");

            foreach (var param in parms)
            {
                preparedQuery.Append($"{param.Key}={ValueToString(param.Value)} ");
            }

            preparedQuery.Append(query);

            return preparedQuery.ToString();
        }

        internal static string ValueToString(object value)
        {
            if (value == null)
            {
                return "null";
            }

            return JsonConvert.SerializeObject(value);
        }

        internal static string QuoteString(string candidateString)
        {
            if (candidateString.StartsWith("\"") && candidateString.EndsWith("\""))
            {
                return candidateString;
            }

            var result = new StringBuilder(candidateString.Length + 2);

            if (!candidateString.StartsWith("\""))
            {
                result.Append('"');
            }

            result.Append(candidateString);

            if (!candidateString.EndsWith("\""))
            {
                result.Append('"');
            }

            return result.ToString();
        }
    }
}