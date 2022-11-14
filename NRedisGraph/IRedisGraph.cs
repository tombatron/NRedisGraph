// .NET port of https://github.com/RedisGraph/JRedisGraph

using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NRedisGraph
{
    /// <summary>
    /// RedisGraph client.
    /// 
    /// This class facilitates querying RedisGraph and parsing the results.
    /// </summary>
    public interface IRedisGraph
    {
        /// <summary>
        /// Execute a Cypher query with parameters.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="parameters">Parameters map.</param>
        /// <returns>A result set.</returns>
        ResultSet GraphQuery(string graphId, string query, IDictionary<string, object> parameters);

        /// <summary>
        /// Execute a Cypher query with parameters.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="parameters">Parameters map.</param>
        /// <returns>A result set.</returns>
        ResultSet Query(string graphId, string query, IDictionary<string, object> parameters);

        /// <summary>
        /// Execute a Cypher query.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <returns>A result set.</returns>
        ResultSet GraphQuery(string graphId, string query);

        /// <summary>
        /// Execute a Cypher query.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <returns>A result set.</returns>
        ResultSet Query(string graphId, string query);

        /// <summary>
        /// Execute a Cypher query with parameters.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="parameters">Parameters map.</param>
        /// <returns>A result set.</returns>
        Task<ResultSet> GraphQueryAsync(string graphId, string query, IDictionary<string, object> parameters);

        /// <summary>
        /// Execute a Cypher query with parameters.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="parameters">Parameters map.</param>
        /// <returns>A result set.</returns>
        Task<ResultSet> QueryAsync(string graphId, string query, IDictionary<string, object> parameters);

        /// <summary>
        /// Execute a Cypher query.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <returns>A result set.</returns>
        Task<ResultSet> GraphQueryAsync(string graphId, string query);

        /// <summary>
        /// Execute a Cypher query.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <returns>A result set.</returns>
        Task<ResultSet> QueryAsync(string graphId, string query);

        /// <summary>
        /// Execute a Cypher query, preferring a read-only node.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="parameters">Parameters map.</param>
        /// <param name="flags">Optional command flags. `PreferReplica` is set for you here.</param>
        /// <returns>A result set.</returns>
        ResultSet GraphReadOnlyQuery(string graphId, string query, IDictionary<string, object> parameters, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Execute a Cypher query, preferring a read-only node.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="flags">Optional command flags. `PreferReplica` is set for you here.</param>
        /// <returns>A result set.</returns>
        ResultSet GraphReadOnlyQuery(string graphId, string query, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Execute a Cypher query, preferring a read-only node.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="parameters">Parameters map.</param>
        /// <param name="flags">Optional command flags. `PreferReplica` is set for you here.</param>
        /// <returns>A result set.</returns>
        Task<ResultSet> GraphReadOnlyQueryAsync(string graphId, string query, IDictionary<string, object> parameters, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Execute a Cypher query, preferring a read-only node.
        /// </summary>
        /// <param name="graphId">A graph to perform the query on.</param>
        /// <param name="query">The Cypher query.</param>
        /// <param name="flags">Optional command flags. `PreferReplica` is set for you here.</param>
        /// <returns>A result set.</returns>
        Task<ResultSet> GraphReadOnlyQueryAsync(string graphId, string query, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Call a saved procedure.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <returns>A result set.</returns>
        ResultSet CallProcedure(string graphId, string procedure);

        /// <summary>
        /// Call a saved procedure.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <returns>A result set.</returns>
        Task<ResultSet> CallProcedureAsync(string graphId, string procedure);

        /// <summary>
        /// Call a saved procedure with parameters.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <returns>A result set.</returns>
        ResultSet CallProcedure(string graphId, string procedure, IEnumerable<string> args);

        /// <summary>
        /// Call a saved procedure with parameters.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <returns>A result set.</returns>
        Task<ResultSet> CallProcedureAsync(string graphId, string procedure, IEnumerable<string> args);

        /// <summary>
        /// Call a saved procedure with parameters.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="kwargs">A collection of keyword arguments.</param>
        /// <returns>A result set.</returns>
        ResultSet CallProcedure(string graphId, string procedure, IEnumerable<string> args, Dictionary<string, List<string>> kwargs);

        /// <summary>
        /// Call a saved procedure with parameters.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="kwargs">A collection of keyword arguments.</param>
        /// <returns>A result set.</returns>
        Task<ResultSet> CallProcedureAsync(string graphId, string procedure, IEnumerable<string> args, Dictionary<string, List<string>> kwargs);

        /// <summary>
        /// Create a RedisGraph transaction.
        /// 
        /// This leverages the "Transaction" support present in StackExchange.Redis.
        /// </summary>
        /// <returns></returns>
        RedisGraphTransaction Multi();

        /// <summary>
        /// Delete an existing graph.
        /// </summary>
        /// <param name="graphId">The graph to delete.</param>
        /// <returns>A result set.</returns>
        ResultSet DeleteGraph(string graphId);

        /// <summary>
        /// Delete an existing graph.
        /// </summary>
        /// <param name="graphId">The graph to delete.</param>
        /// <returns>A result set.</returns>
        Task<ResultSet> DeleteGraphAsync(string graphId);

        /// <summary>
        /// Call a saved procedure against a read-only node.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <returns>A result set.</returns>
        ResultSet CallProcedureReadOnly(string graphId, string procedure);

        /// <summary>
        /// Call a saved procedure against a read-only node.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <returns>A result set.</returns>
        Task<ResultSet> CallProcedureReadOnlyAsync(string graphId, string procedure);

        /// <summary>
        /// Call a saved procedure with parameters against a read-only node.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <returns>A result set.</returns>
        ResultSet CallProcedureReadOnly(string graphId, string procedure, IEnumerable<string> args);

        /// <summary>
        /// Call a saved procedure with parameters against a read-only node.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <returns>A result set.</returns>
        Task<ResultSet> CallProcedureReadOnlyAsync(string graphId, string procedure, IEnumerable<string> args);

        /// <summary>
        /// Call a saved procedure with parameters against a read-only node.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="kwargs">A collection of keyword arguments.</param>
        /// <returns>A result set.</returns>
        ResultSet CallProcedureReadOnly(string graphId, string procedure, IEnumerable<string> args, Dictionary<string, List<string>> kwargs);

        /// <summary>
        /// Call a saved procedure with parameters against a read-only node.
        /// </summary>
        /// <param name="graphId">The graph containing the saved procedure.</param>
        /// <param name="procedure">The procedure name.</param>
        /// <param name="args">A collection of positional arguments.</param>
        /// <param name="kwargs">A collection of keyword arguments.</param>
        /// <returns>A result set.</returns>
        Task<ResultSet> CallProcedureReadOnlyAsync(string graphId, string procedure, IEnumerable<string> args, Dictionary<string, List<string>> kwargs);
    }
}