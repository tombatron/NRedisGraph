namespace NRedisGraph
{
    public sealed class Statistics
    {
        public enum Label
        {
            LABELS_ADDED,
            INDICES_ADDED,
            NODES_CREATED,
            NODES_DELETED,
            RELATIONSHIPS_DELETED,
            PROPERTIES_SET,
            RELATIONSHIPS_CREATED,
            QUERY_INTERNAL_EXECUTION_TIME
        }

        public string GetStringValue(Label label)
        {
            return default;
        }

        public int NodesCreated { get; }

        public int NodesDeleted { get; }

        public int IndicesAdded { get; }

        public int LabelsAdded { get; }

        public int RelationshipsDeleted { get; }

        public int RelationshipsCreated { get; }

        public int PropertiesSet { get; }
    }
}