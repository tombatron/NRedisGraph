// .NET port of https://github.com/RedisGraph/JRedisGraph

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using static NRedisGraph.RedisGraphUtilities;

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
        private readonly IDictionary<string, IGraphCache> _graphCaches = new Dictionary<string, IGraphCache>();

        private IGraphCache GetGraphCache(string graphId)
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
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param> 
        /// <returns>A result set.</returns>
        public ResultSet GraphQuery(string graphId, string query, IDictionary<string, object> parameters, CommandFlags flags = CommandFlags.None) =>
            Query(graphId, query, parameters, flags);

        /// <summary>
        /// Execute a Cypher query with parameters.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="parameters">Parameters map.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>
        /// <returns>A result set.</returns>
        public ResultSet Query(string graphId, string query, IDictionary<string, object> parameters, CommandFlags flags = CommandFlags.None)
        {
            var preparedQuery = PrepareQuery(query, parameters);

            return Query(graphId, preparedQuery, flags);
        }

        /// <summary>
        /// Execute a Cypher query.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>
        /// <returns>A result set.</returns>
        public ResultSet GraphQuery(string graphId, string query, CommandFlags flags = CommandFlags.None) =>
            Query(graphId, query, flags);

        /// <summary>
        /// Execute a Cypher query.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>
        /// <returns>A result set.</returns>
        public ResultSet Query(string graphId, string query, CommandFlags flags = CommandFlags.None)
        {
            _graphCaches.PutIfAbsent(graphId, new GraphCache(graphId, this));

            var commandArgs = new object[]
            {
                graphId,
                query,
                CompactQueryFlag
            };

            var rawResult = _db.Execute(Command.QUERY, commandArgs, flags);

            if (flags.HasFlag(CommandFlags.FireAndForget))
            {
                return default;
            }
            else
            {
                return new ResultSet(rawResult, _graphCaches[graphId]);
            }
        }

        /// <summary>
        /// Execute a Cypher query with parameters.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="parameters">Parameters map.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>
        /// <returns>A result set.</returns>
        public Task<ResultSet> GraphQueryAsync(string graphId, string query, IDictionary<string, object> parameters, CommandFlags flags = CommandFlags.None) =>
            QueryAsync(graphId, query, parameters, flags);

        /// <summary>
        /// Execute a Cypher query with parameters.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="parameters">Parameters map.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>
        /// <returns>A result set.</returns>
        public Task<ResultSet> QueryAsync(string graphId, string query, IDictionary<string, object> parameters, CommandFlags flags = CommandFlags.None)
        {
            var preparedQuery = PrepareQuery(query, parameters);

            return QueryAsync(graphId, preparedQuery, flags);
        }

        /// <summary>
        /// Execute a Cypher query.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>
        /// <returns>A result set.</returns>
        public Task<ResultSet> GraphQueryAsync(string graphId, string query, CommandFlags flags = CommandFlags.None) =>
            QueryAsync(graphId, query, flags);

        /// <summary>
        /// Execute a Cypher query.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>
        /// <returns>A result set.</returns>
        public async Task<ResultSet> QueryAsync(string graphId, string query, CommandFlags flags = CommandFlags.None)
        {
            _graphCaches.PutIfAbsent(graphId, new GraphCache(graphId, this));

            var commandArgs = new object[]
            {
                graphId,
                query,
                CompactQueryFlag
            };

            var rawResult = await _db.ExecuteAsync(Command.QUERY, commandArgs, flags);

            if (flags.HasFlag(CommandFlags.FireAndForget))
            {
                return default;
            }
            else
            {
                return new ResultSet(rawResult, _graphCaches[graphId]);
            }
        }

        /// <summary>
        /// Execute a Cypher query, preferring a read-only node.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="parameters">Parameters map.</param>
        /// <param name="flags">Optional command flags. `PreferReplica` is set for you here.</param>
        /// <returns>A result set.</returns>
        public ResultSet GraphReadOnlyQuery(string graphId, string query, IDictionary<string, object> parameters, CommandFlags flags = CommandFlags.None)
        {
            var preparedQuery = PrepareQuery(query, parameters);

            return GraphReadOnlyQuery(graphId, preparedQuery, flags);
        }

        /// <summary>
        /// Execute a Cypher query, preferring a read-only node.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="flags">Optional command flags. `PreferReplica` is set for you here.</param>
        /// <returns>A result set.</returns>
        public ResultSet GraphReadOnlyQuery(string graphId, string query, CommandFlags flags = CommandFlags.None)
        {
            _graphCaches.PutIfAbsent(graphId, new ReadOnlyGraphCache(graphId, this));

            var parameters = new object[]
            {
                graphId,
                query,
                CompactQueryFlag
            };

            var result = _db.Execute(Command.RO_QUERY, parameters, (flags | CommandFlags.PreferReplica));

            return new ResultSet(result, _graphCaches[graphId]);
        }

        /// <summary>
        /// Execute a Cypher query, preferring a read-only node.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="parameters">Parameters map.</param>
        /// <param name="flags">Optional command flags. `PreferReplica` is set for you here.</param>
        /// <returns>A result set.</returns>
        public Task<ResultSet> GraphReadOnlyQueryAsync(string graphId, string query, IDictionary<string, object> parameters, CommandFlags flags = CommandFlags.None)
        {
            var preparedQuery = PrepareQuery(query, parameters);

            return GraphReadOnlyQueryAsync(graphId, preparedQuery, flags);
        }

        /// <summary>
        /// Execute a Cypher query, preferring a read-only node.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="flags">Optional command flags. `PreferReplica` is set for you here.</param>
        /// <returns>A result set.</returns>
        public async Task<ResultSet> GraphReadOnlyQueryAsync(string graphId, string query, CommandFlags flags = CommandFlags.None)
        {
            _graphCaches.PutIfAbsent(graphId, new ReadOnlyGraphCache(graphId, this));

            var parameters = new object[]
            {
                graphId,
                query,
                CompactQueryFlag
            };

            var result = await _db.ExecuteAsync(Command.RO_QUERY, parameters, (flags | CommandFlags.PreferReplica));

            return new ResultSet(result, _graphCaches[graphId]);
        }

        internal static readonly Dictionary<string, List<string>> EmptyKwargsDictionary =
            new Dictionary<string, List<string>>();

        /// <summary>
        /// Call a saved procedure.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public ResultSet CallProcedure(string graphId, string procedure, CommandFlags flags = CommandFlags.None) =>
            CallProcedure(graphId, procedure, Enumerable.Empty<string>(), EmptyKwargsDictionary, flags);

        /// <summary>
        /// Call a saved procedure.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public Task<ResultSet> CallProcedureAsync(string graphId, string procedure, CommandFlags flags = CommandFlags.None) =>
            CallProcedureAsync(graphId, procedure, Enumerable.Empty<string>(), EmptyKwargsDictionary, flags);

        /// <summary>
        /// Call a saved procedure with parameters.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public ResultSet CallProcedure(string graphId, string procedure, IEnumerable<string> args, CommandFlags flags = CommandFlags.None) =>
            CallProcedure(graphId, procedure, args, EmptyKwargsDictionary, flags);

        /// <summary>
        /// Call a saved procedure with parameters.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public Task<ResultSet> CallProcedureAsync(string graphId, string procedure, IEnumerable<string> args, CommandFlags flags = CommandFlags.None) =>
            CallProcedureAsync(graphId, procedure, args, EmptyKwargsDictionary);

        /// <summary>
        /// Call a saved procedure with parameters.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="kwargs">A collection of keyword arguments.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public ResultSet CallProcedure(string graphId, string procedure, IEnumerable<string> args, Dictionary<string, List<string>> kwargs, CommandFlags flags = CommandFlags.None)
        {
            args = args.Select(a => QuoteString(a));

            var queryBody = new StringBuilder();

            queryBody.Append($"CALL {procedure}({string.Join(",", args)})");

            if (kwargs.TryGetValue("y", out var kwargsList))
            {
                queryBody.Append(string.Join(",", kwargsList));
            }

            return Query(graphId, queryBody.ToString(), flags);
        }

        /// <summary>
        /// Call a saved procedure with parameters.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="kwargs">A collection of keyword arguments.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public Task<ResultSet> CallProcedureAsync(string graphId, string procedure, IEnumerable<string> args, Dictionary<string, List<string>> kwargs, CommandFlags flags = CommandFlags.None)
        {
            args = args.Select(a => QuoteString(a));

            var queryBody = new StringBuilder();

            queryBody.Append($"CALL {procedure}({string.Join(",", args)})");

            if (kwargs.TryGetValue("y", out var kwargsList))
            {
                queryBody.Append(string.Join(",", kwargsList));
            }

            return QueryAsync(graphId, queryBody.ToString(), flags);
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
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public ResultSet DeleteGraph(string graphId, CommandFlags flags = CommandFlags.None)
        {
            var commandArgs = new object[]
            {
                graphId
            };

            var rawResult = _db.Execute(Command.DELETE, commandArgs, flags);

            _graphCaches.Remove(graphId);

            if (flags.HasFlag(CommandFlags.FireAndForget))
            {
                return default;
            }
            else
            {
                return new ResultSet(rawResult, _graphCaches[graphId]);
            }
        }

        /// <summary>
        /// Delete an existing graph.
        /// </summary>
        /// <param name="graphId">The graph to delete.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public async Task<ResultSet> DeleteGraphAsync(string graphId, CommandFlags flags = CommandFlags.None)
        {
            var commandArgs = new object[]
            {
                graphId
            };

            var rawResult = await _db.ExecuteAsync(Command.DELETE, commandArgs, flags);

            _graphCaches.Remove(graphId);

            if (flags.HasFlag(CommandFlags.FireAndForget))
            {
                return default;
            }
            else
            {
                return new ResultSet(rawResult, _graphCaches[graphId]);
            }
        }

        /// <summary>
        /// Call a saved procedure against a read-only node.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public ResultSet CallProcedureReadOnly(string graphId, string procedure, CommandFlags flags = CommandFlags.None) =>
            CallProcedureReadOnly(graphId, procedure, Enumerable.Empty<string>(), EmptyKwargsDictionary, flags);

        /// <summary>
        /// Call a saved procedure against a read-only node.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public Task<ResultSet> CallProcedureReadOnlyAsync(string graphId, string procedure, CommandFlags flags = CommandFlags.None) =>
            CallProcedureReadOnlyAsync(graphId, procedure, Enumerable.Empty<string>(), EmptyKwargsDictionary, flags);

        /// <summary>
        /// Call a saved procedure with parameters against a read-only node.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public ResultSet CallProcedureReadOnly(string graphId, string procedure, IEnumerable<string> args, CommandFlags flags = CommandFlags.None) =>
            CallProcedureReadOnly(graphId, procedure, args, EmptyKwargsDictionary, flags);

        /// <summary>
        /// Call a saved procedure with parameters against a read-only node.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public Task<ResultSet> CallProcedureReadOnlyAsync(string graphId, string procedure, IEnumerable<string> args, CommandFlags flags = CommandFlags.None) =>
            CallProcedureReadOnlyAsync(graphId, procedure, args, EmptyKwargsDictionary, flags);

        /// <summary>
        /// Call a saved procedure with parameters against a read-only node.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="kwargs">A collection of keyword arguments.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public ResultSet CallProcedureReadOnly(string graphId, string procedure, IEnumerable<string> args, Dictionary<string, List<string>> kwargs, CommandFlags flags = CommandFlags.None)
        {
            args = args.Select(a => QuoteString(a));

            var queryBody = new StringBuilder();

            queryBody.Append($"CALL {procedure}({string.Join(",", args)})");

            if (kwargs.TryGetValue("y", out var kwargsList))
            {
                queryBody.Append(string.Join(",", kwargsList));
            }

            return GraphReadOnlyQuery(graphId, queryBody.ToString(), flags);
        }

        /// <summary>
        /// Call a saved procedure with parameters against a read-only node.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="kwargs">A collection of keyword arguments.</param>
        /// <param name="flags">[Optional] Command flags that are to be sent to the StackExchange.Redis connection multiplexer...</param>/// 
        /// <returns>A result set.</returns>
        public Task<ResultSet> CallProcedureReadOnlyAsync(string graphId, string procedure, IEnumerable<string> args, Dictionary<string, List<string>> kwargs, CommandFlags flags = CommandFlags.None)
        {
            args = args.Select(a => QuoteString(a));

            var queryBody = new StringBuilder();

            queryBody.Append($"CALL {procedure}({string.Join(",", args)})");

            if (kwargs.TryGetValue("y", out var kwargsList))
            {
                queryBody.Append(string.Join(",", kwargsList));
            }

            return GraphReadOnlyQueryAsync(graphId, queryBody.ToString(), flags);
        }
    }
}