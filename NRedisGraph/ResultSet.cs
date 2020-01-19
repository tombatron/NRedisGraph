using System.Linq;
using System.Collections;
using System.Collections.Generic;
using StackExchange.Redis;

namespace NRedisGraph
{
    public sealed class ResultSet : IReadOnlyCollection<Record>
    {
        public enum ResultSetScalarType
        {
            PROPERTY_UNKNOWN,
            PROPERTY_NULL,
            PROPERTY_STRING,
            PROPERTY_INTEGER,
            PROPERTY_BOOLEAN,
            PROPERTY_DOUBLE
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
                                parsedRow.Add(DeserializedNode(obj));
                                break;
                            case Header.ResultSetColumnTypes.COLUMN_RELATION:
                                parsedRow.Add(DeserializedEdge(obj));
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

        private Node DeserializedNode(RedisResult[] rawNodeData)
        {
            var node = new Node();

            DeserializeGraphEntityId(node, rawNodeData[0]);

            var labelIndices = (int[])rawNodeData[1];

            foreach(var labelIndex in labelIndices)
            {
                var label = _graphCache.GetLabel(labelIndex);

                node.AddLabel(label);
            }

            DeserializeGraphEntityProperties(node, (RedisResult[]) rawNodeData[2]);

            return node;
        }

        private Edge DeserializedEdge(RedisResult[] rawEdgeData)
        {
            return default;
        }

        private object DeserializeScalar(RedisResult[] rawScalarData)
        {
            var type = GetValueTypeFromObject(rawScalarData[0]);

            switch(type)
            {
                case ResultSetScalarType.PROPERTY_NULL:
                    return null;
                case ResultSetScalarType.PROPERTY_BOOLEAN:
                    return (bool)rawScalarData[1];
                case ResultSetScalarType.PROPERTY_DOUBLE:
                    return (double)rawScalarData[1];
                case ResultSetScalarType.PROPERTY_INTEGER:
                    return (int)rawScalarData[1];
                case ResultSetScalarType.PROPERTY_STRING:
                    return (string)rawScalarData[1];
                case ResultSetScalarType.
                default:
                    return (object)rawScalarData[1];
            }
        }

        private static void DeserializeGraphEntityId(GraphEntity graphEntity, object rawEntityId) =>
            graphEntity.Id = (int)rawEntityId;

        private void DeserializeGraphEntityProperties(GraphEntity graphEntity, RedisResult[] rawProperties)
        {
            foreach(RedisResult[] rawProperty in rawProperties)
            {
                var property = new Property
                {
                    Name = _graphCache.GetPropertyName((int)rawProperty[0]),
                    Value = DeserializeScalar(rawProperty.Skip(1).ToArray())
                };

                graphEntity.AddProperty(property);
            }
        }

        private static ResultSetScalarType GetValueTypeFromObject(RedisResult rawScalarType) =>
            (ResultSetScalarType)(int)rawScalarType;
    }
}