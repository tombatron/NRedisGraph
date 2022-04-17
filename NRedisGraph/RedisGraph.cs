// .NET port of https://github.com/RedisGraph/JRedisGraph
using StackExchange.Redis;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ResultSet GraphQuery(string graphId, string query, IDictionary<string, object> parameters) =>
            Query(graphId, query, parameters);
            
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
        public ResultSet GraphQuery(string graphId, string query) =>
            Query(graphId, query);

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
        public Task<ResultSet> GraphQueryAsync(string graphId, string query, IDictionary<string, object> parameters) =>
            QueryAsync(graphId, query, parameters);

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
        public Task<ResultSet> GraphQueryAsync(string graphId, string query) =>
            QueryAsync(graphId, query);

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
            _graphCaches.PutIfAbsent(graphId, new GraphCache(graphId, this));

            var parameters = new Collection<object>
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
            _graphCaches.PutIfAbsent(graphId, new GraphCache(graphId, this));

            var parameters = new Collection<object>
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

            if (value is string stringValue)
            {
                return QuoteString(stringValue);
            }

            if (value is char charValue)
            {
                return QuoteCharacter(charValue);
            }

            if (value.GetType().IsArray)
            {
                if (value is IEnumerable arrayValue)
                {
                    var values = new List<object>();

                    foreach (var v in arrayValue)
                    {
                        values.Add(v);
                    }

                    return ArrayToString(values.ToArray());
                }
            }

            if ((value is System.Collections.IList valueList) && value.GetType().IsGenericType)
            {
                var objectValueList = new List<object>();

                foreach (var val in valueList)
                {
                    objectValueList.Add((object)val);
                }

                return ArrayToString(objectValueList.ToArray());
            }

            if (value is bool boolValue)
            {
                return boolValue.ToString().ToLowerInvariant();
            }

            return value.ToString();
        }

        private static string ArrayToString(object[] array)
        {
            var arrayElements = array.Select(x =>
            {
                if (x.GetType().IsArray)
                {
                    return ArrayToString((object[])x);
                }
                else
                {
                    return ValueToString(x);
                }
            });

            var arrayToString = new StringBuilder();

            arrayToString.Append('[');
            arrayToString.Append(string.Join(", ", arrayElements));
            arrayToString.Append(']');

            return arrayToString.ToString();
        }

        internal static string QuoteCharacter(char character) =>
            $"\"{character}\"";

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

            //result.Append(candidateString);
            foreach(var c in candidateString)
            {
                if (c == '"')
                {
                    result.Append("\\");
                    
                }

                result.Append(c);
            }

            if (!candidateString.EndsWith("\""))
            {
                result.Append('"');
            }

            return result.ToString();
        }
    }
}