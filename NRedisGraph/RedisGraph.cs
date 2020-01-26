// .NET port of https://github.com/RedisGraph/JRedisGraph
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace NRedisGraph
{
    public sealed class RedisGraph
    {
        private static readonly object CompactQueryFlag = "--COMPACT";
        private readonly IDatabase _db;
        private readonly IDictionary<string, GraphCache> _graphCaches = new Dictionary<string, GraphCache>();

        public GraphCache GetGraphCache(string graphId)
        {
            if (!_graphCaches.ContainsKey(graphId))
            {
                _graphCaches.Add(graphId, new GraphCache(graphId, this));
            }

            return _graphCaches[graphId];
        }

        public RedisGraph(IDatabase db) => _db = db;

        public ResultSet Query(string graphId, string query, IDictionary<string, object> parameters)
        {
            var preparedQuery = PrepareQuery(query, parameters);

            return Query(graphId, preparedQuery);
        }

        public ResultSet Query(string graphId, string query)
        {
            _graphCaches.PutIfAbsent(graphId, new GraphCache(graphId, this));

            return new ResultSet(_db.Execute(Command.QUERY, graphId, query, CompactQueryFlag), _graphCaches[graphId]);
        }

        public Task<ResultSet> QueryAsync(string graphId, string query, IDictionary<string, object> parameters)
        {
            var preparedQuery = PrepareQuery(query, parameters);

            return QueryAsync(graphId, preparedQuery);
        }

        public async Task<ResultSet> QueryAsync(string graphId, string query)
        {
            _graphCaches.PutIfAbsent(graphId, new GraphCache(graphId, this));

            return new ResultSet(await _db.ExecuteAsync(Command.QUERY, graphId, query, CompactQueryFlag), _graphCaches[graphId]);
        }

        private static readonly Dictionary<string, List<string>> EmptyKwargsDictionary =
            new Dictionary<string, List<string>>();

        public ResultSet CallProcedure(string graphId, string procedure) =>
            CallProcedure(graphId, procedure, Enumerable.Empty<string>(), EmptyKwargsDictionary);

        public Task<ResultSet> CallProcedureAsync(string graphId, string procedure) =>
            CallProcedureAsync(graphId, procedure, Enumerable.Empty<string>(), EmptyKwargsDictionary);

        public ResultSet CallProcedure(string graphId, string procedure, IEnumerable<string> args) =>
            CallProcedure(graphId, procedure, args, EmptyKwargsDictionary);

        public Task<ResultSet> CallProcedureAsync(string graphId, string procedure, IEnumerable<string> args) =>
            CallProcedureAsync(graphId, procedure, args, EmptyKwargsDictionary);

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

        public static string PrepareQuery(string query, IDictionary<string, object> parms)
        {
            var preparedQuery = new StringBuilder();

            preparedQuery.Append("CYPHER ");

            foreach(var param in parms)
            {
                preparedQuery.Append($"{param.Key}={ValueToString(param.Value)} ");
            }

            preparedQuery.Append(query);

            return preparedQuery.ToString();
        }

        private static string ValueToString(object value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is string stringValue)
            {
                // return EscapeQuotes(stringValue);
                return QuoteString(stringValue);
            }

            if (value.GetType().IsArray)
            {
                return ArrayToString((object[])value);
            }

            if ((value is System.Collections.IList valueList) && value.GetType().IsGenericType)
            {
                return ArrayToString(((List<object>)valueList).ToArray());
            }

            if (value is bool boolValue)
            {
                return boolValue.ToString().ToLowerInvariant();
            }

            return value.ToString();
        }

        private static string ArrayToString(object[] array)
        {
                var arrayToString = new StringBuilder();

                arrayToString.Append('[');
                arrayToString.Append(string.Join(", ", array.Select(x=>x.ToString())));
                arrayToString.Append(']');

                return arrayToString.ToString();
        }

        private static string EscapeQuotes(string unescapedString)
        {
            int replacementCount = 0;

            for (var i = 0; i < unescapedString.Length; i++)
            {
                var currentCharacter = unescapedString[i];

                if (currentCharacter == '\'' || currentCharacter == '"')
                {
                    if (i == 0 || unescapedString[i - 1] != '\\')
                    {
                        replacementCount++;
                    }
                }
            }

            if (replacementCount == 0)
            {
                return unescapedString;
            }

            var result = new StringBuilder(unescapedString + replacementCount);

            for (var i = 0; i < unescapedString.Length; i++)
            {
                var currentCharacter = unescapedString[i];

                if (currentCharacter == '\'' || currentCharacter == '"')
                {
                    if (i == 0 || unescapedString[i - 1] != '\\')
                    {
                        result.Append('\\');
                    }
                }

                result.Append(currentCharacter);
            }

            return result.ToString();
        }

        private static string QuoteString(string candidateString)
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