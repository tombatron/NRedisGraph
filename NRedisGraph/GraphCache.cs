namespace NRedisGraph
{
    public sealed class GraphCache
    {
        private readonly GraphCacheList _labels;
        private readonly GraphCacheList _propertyNames;
        private readonly GraphCacheList _relationshipTypes;

        public GraphCache(string graphId, RedisGraph redisGraph)
        {
            _labels = new GraphCacheList(graphId, "db.labels", redisGraph);
            _propertyNames = new GraphCacheList(graphId, "db.propertyKeys", redisGraph);
            _relationshipTypes = new GraphCacheList(graphId, "db.relationshipTypes", redisGraph);
        }

        public string GetLabel(int index) => _labels.GetCachedData(index);

        public string GetRelationshipType(int index) => _relationshipTypes.GetCachedData(index);

        public string GetPropertyName(int index) => _propertyNames.GetCachedData(index);
    }
}