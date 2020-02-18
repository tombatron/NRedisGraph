using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace NRedisGraph
{
    public sealed class Statistics
    {
        public class Label
        {

            private const string LABELS_ADDED = "Labels added";
            private const string INDICES_ADDED = "Indices added";
            private const string INDICES_CREATED = "Indices created";
            private const string NODES_CREATED = "Nodes created";
            private const string NODES_DELETED = "Nodes deleted";
            private const string RELATIONSHIPS_DELETED = "Relationships deleted";
            private const string PROPERTIES_SET = "Properties set";
            private const string RELATIONSHIPS_CREATED = "Relationships created";
            private const string QUERY_INTERNAL_EXECUTION_TIME = "Query internal execution time";
            private const string GRAPH_REMOVED_INTERNAL_EXECUTION_TIME = "Graph removed, internal execution time";

            public string Value { get; }

            private Label(string value) => Value = value;

            public static readonly Label LabelsAdded = new Label(LABELS_ADDED);
            public static readonly Label IndicesAdded = new Label(INDICES_ADDED);
            public static readonly Label IndicesCreated = new Label(INDICES_CREATED);
            public static readonly Label NodesCreated = new Label(NODES_CREATED);
            public static readonly Label NodesDeleted = new Label(NODES_DELETED);
            public static readonly Label RelationshipsDeleted = new Label(RELATIONSHIPS_DELETED);
            public static readonly Label PropertiesSet = new Label(PROPERTIES_SET);
            public static readonly Label RelationshipsCreated = new Label(RELATIONSHIPS_CREATED);
            public static readonly Label QueryInternalExecutionTime = new Label(QUERY_INTERNAL_EXECUTION_TIME);
            public static readonly Label GraphRemovedInternalExecutionTime = new Label(GRAPH_REMOVED_INTERNAL_EXECUTION_TIME);

            public static Label FromString(string labelValue)
            {
                switch (labelValue)
                {
                    case LABELS_ADDED:
                        return LabelsAdded;
                    case INDICES_ADDED:
                        return IndicesAdded;
                    case INDICES_CREATED:
                        return IndicesCreated;
                    case NODES_CREATED:
                        return NodesCreated;
                    case NODES_DELETED:
                        return NodesDeleted;
                    case RELATIONSHIPS_DELETED:
                        return RelationshipsDeleted;
                    case PROPERTIES_SET:
                        return PropertiesSet;
                    case RELATIONSHIPS_CREATED:
                        return RelationshipsCreated;
                    case QUERY_INTERNAL_EXECUTION_TIME:
                        return QueryInternalExecutionTime;
                    case GRAPH_REMOVED_INTERNAL_EXECUTION_TIME:
                        return GraphRemovedInternalExecutionTime;
                    default:
                        throw new ArgumentException("Unknown label kind.", nameof(labelValue));
                }
            }
        }

        private readonly RedisResult[] _statistics;

        internal Statistics(RedisResult statistics)
        {
            if (statistics.Type == ResultType.MultiBulk)
            {
                _statistics = (RedisResult[])statistics;
            }
            else
            {
                _statistics = new[] { statistics };
            }
        }


        private IDictionary<Label, string> _statisticsValues;

        public string GetStringValue(Label label)
        {
            if (_statisticsValues == default)
            {
                _statisticsValues = _statistics
                    .Select(x =>
                    {
                        var s = ((string)x).Split(':');

                        return new
                        {
                            Label = Label.FromString(s[0].Trim()),
                            Value = s[1].Trim()
                        };
                    }).ToDictionary(k => k.Label, v => v.Value);
            }

            return _statisticsValues.TryGetValue(label, out var value) ? value : default;
        }

        public int NodesCreated => int.TryParse(GetStringValue(Label.NodesCreated), out var result) ? result : 0;

        public int NodesDeleted => int.TryParse(GetStringValue(Label.NodesDeleted), out var result) ? result : 0;

        public int IndicesAdded => int.TryParse(GetStringValue(Label.IndicesAdded), out var result) ? result : 0;

        public int IndicesCreated => int.TryParse(GetStringValue(Label.IndicesCreated), out var result) ? result : 0;

        public int LabelsAdded => int.TryParse(GetStringValue(Label.LabelsAdded), out var result) ? result : 0;

        public int RelationshipsDeleted => int.TryParse(GetStringValue(Label.RelationshipsDeleted), out var result) ? result : 0;

        public int RelationshipsCreated => int.TryParse(GetStringValue(Label.RelationshipsCreated), out var result) ? result : 0;

        public int PropertiesSet => int.TryParse(GetStringValue(Label.PropertiesSet), out var result) ? result : 0;

        public string QueryInternalExecutionTime => GetStringValue(Label.QueryInternalExecutionTime);

        public string GraphRemovedInternalExecutionTime => GetStringValue(Label.GraphRemovedInternalExecutionTime);
    }
}