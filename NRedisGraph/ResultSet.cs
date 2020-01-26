using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace NRedisGraph
{
    public sealed class ResultSet : IReadOnlyCollection<Record>
    {
        public enum ResultSetScalarType
        {
            VALUE_UNKNOWN,
            VALUE_NULL,
            VALUE_STRING,
            VALUE_INTEGER,
            VALUE_BOOLEAN,
            VALUE_DOUBLE,
            VALUE_ARRAY,
            VALUE_EDGE,
            VALUE_NODE,
            VALUE_PATH
        }

        private readonly RedisResult[] _rawResults;
        private readonly GraphCache _graphCache;

        public ResultSet(RedisResult result, GraphCache graphCache)
        {
            var resultArray = (RedisResult[])result;
            _graphCache = graphCache;

            if (resultArray.Length == 3)
            {
                Header = new Header(resultArray[0]);
                Statistics = new Statistics(resultArray[2]);

                _rawResults = (RedisResult[])resultArray[1];

                Count = _rawResults.Length;
            }
            else
            {
                Statistics = new Statistics(resultArray[resultArray.Length - 1]);
                Count = 0;
            }
        }

        public Statistics Statistics { get; }

        public Header Header { get; }

        public int Count { get; }

        public IEnumerator<Record> GetEnumerator() => RecordIterator().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => RecordIterator().GetEnumerator();

        private IEnumerable<Record> RecordIterator()
        {
            if (_rawResults == default)
            {
                yield break;
            }
            else
            {
                foreach (RedisResult[] row in _rawResults)
                {
                    var parsedRow = new List<object>(row.Length);

                    for (int i = 0; i < row.Length; i++)
                    {
                        var obj = (RedisResult[])row[i];
                        var objType = Header.SchemaTypes[i];

                        switch (objType)
                        {
                            case Header.ResultSetColumnTypes.COLUMN_NODE:
                                parsedRow.Add(DeserializeNode(obj));
                                break;
                            case Header.ResultSetColumnTypes.COLUMN_RELATION:
                                parsedRow.Add(DeserializeEdge(obj));
                                break;
                            case Header.ResultSetColumnTypes.COLUMN_SCALAR:
                                parsedRow.Add(DeserializeScalar(obj));
                                break;
                            default:
                                parsedRow.Add(null);
                                break;
                        }
                    }

                    yield return new Record(Header.SchemaNames, parsedRow);
                }

                yield break;
            }
        }

        private Node DeserializeNode(RedisResult[] rawNodeData)
        {
            var node = new Node();

            DeserializeGraphEntityId(node, rawNodeData[0]);

            var labelIndices = (int[])rawNodeData[1];

            foreach (var labelIndex in labelIndices)
            {
                var label = _graphCache.GetLabel(labelIndex);

                node.AddLabel(label);
            }

            DeserializeGraphEntityProperties(node, (RedisResult[])rawNodeData[2]);

            return node;
        }

        private Edge DeserializeEdge(RedisResult[] rawEdgeData)
        {
            var edge = new Edge();

            DeserializeGraphEntityId(edge, rawEdgeData[0]);

            edge.RelationshipType = _graphCache.GetRelationshipType((int)rawEdgeData[1]);
            edge.Source = (int)rawEdgeData[2];
            edge.Destination = (int)rawEdgeData[3];

            DeserializeGraphEntityProperties(edge, (RedisResult[])rawEdgeData[4]);

            return edge;
        }

        private object DeserializeScalar(RedisResult[] rawScalarData)
        {
            var type = GetValueTypeFromObject(rawScalarData[0]);

            switch (type)
            {
                case ResultSetScalarType.VALUE_NULL:
                    return null;
                case ResultSetScalarType.VALUE_BOOLEAN:
                    return bool.Parse((string)rawScalarData[1]);
                case ResultSetScalarType.VALUE_DOUBLE:
                    return (double)rawScalarData[1];
                case ResultSetScalarType.VALUE_INTEGER:
                    return (int)rawScalarData[1];
                case ResultSetScalarType.VALUE_STRING:
                    return (string)rawScalarData[1];
                case ResultSetScalarType.VALUE_ARRAY:
                    return DeserializeArray((RedisResult[])rawScalarData[1]);
                case ResultSetScalarType.VALUE_NODE:
                    return DeserializeNode((RedisResult[])rawScalarData[1]);
                case ResultSetScalarType.VALUE_EDGE:
                    return DeserializeEdge((RedisResult[])rawScalarData[1]);
                case ResultSetScalarType.VALUE_PATH:
                    return DeserializePath((RedisResult[])rawScalarData[1]);
                case ResultSetScalarType.VALUE_UNKNOWN:
                default:
                    return (object)rawScalarData[1];
            }
        }

        private static void DeserializeGraphEntityId(GraphEntity graphEntity, RedisResult rawEntityId) =>
            graphEntity.Id = (int)rawEntityId;

        private void DeserializeGraphEntityProperties(GraphEntity graphEntity, RedisResult[] rawProperties)
        {
            foreach (RedisResult[] rawProperty in rawProperties)
            {
                var property = new Property
                {
                    Name = _graphCache.GetPropertyName((int)rawProperty[0]),
                    Value = DeserializeScalar(rawProperty.Skip(1).ToArray())
                };

                graphEntity.AddProperty(property);
            }
        }

        private object[] DeserializeArray(RedisResult[] serializedArray)
        {
            var result = new object[serializedArray.Length];

            for (var i = 0; i < serializedArray.Length; i++)
            {
                result[0] = DeserializeScalar((RedisResult[])serializedArray[i]);
            }

            return result;
        }

        private Path DeserializePath(RedisResult[] rawPath)
        {
            var nodes = new List<Node>((Node[])DeserializeScalar((RedisResult[])rawPath[0]));
            var edges = new List<Edge>((Edge[])DeserializeScalar((RedisResult[])rawPath[1]));

            return new Path(nodes, edges);
        }

        private static ResultSetScalarType GetValueTypeFromObject(RedisResult rawScalarType) =>
        (ResultSetScalarType)(int)rawScalarType;
    }
}