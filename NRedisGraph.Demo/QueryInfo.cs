using System;

namespace NRedisGraph.Demo
{
    public class QueryInfo
    {
        public string Query { get; set; }

        public string Description { get; set; }

        public TimeSpan MaxRuntime { get; set; }

        public object ExpectedResult { get; set; }
    }
}