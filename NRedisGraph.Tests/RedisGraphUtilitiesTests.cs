using Xunit;
using System.Collections.Generic;

namespace NRedisGraph.Tests;

public class RedisGraphUtilitiesTests
{
    public class PrepareQueryWill
    {
        [Theory]
        [MemberData(nameof(ParameterSamples))]
        public void ProperlyFormatParameters(IDictionary<string, object> parameters, string expectedQuery)
        {
            var preparedQuery = RedisGraphUtilities.PrepareQuery("RETURN $param", parameters);

            Assert.Equal(expectedQuery, preparedQuery);
        }

        public static object[][] ParameterSamples => new[]
        {
            new object[]
            {
                new Dictionary<string, object> {{"param", string.Empty}},
                "CYPHER param=\"\" RETURN $param"
            },
            
            new object[]
            {
                new Dictionary<string, object> {{"param", "\""}},
                "CYPHER param=\"\\\"\" RETURN $param"
            },
            
            new object[]
            {
                new Dictionary<string, object> {{"param", "\"st"}},
                "CYPHER param=\"\\\"st\" RETURN $param"
            },
            
            new object[]
            {
                new Dictionary<string, object> {{"param", 1}},
                "CYPHER param=1 RETURN $param"
            },
            
            new object[]
            {
                new Dictionary<string, object> {{"param", 2.3}},
                "CYPHER param=2.3 RETURN $param"
            },

            new object[]
            {
                new Dictionary<string, object> {{"param", true}},
                "CYPHER param=true RETURN $param"
            },

            new object[]
            {
                new Dictionary<string, object> {{"param", false}},
                "CYPHER param=false RETURN $param"
            },

            new object[]
            {
                new Dictionary<string, object> {{"param", null}},
                "CYPHER param=null RETURN $param"
            },
            
            new object[]
            {
                new Dictionary<string, object> {{"param", "str"}},
                "CYPHER param=\"str\" RETURN $param"
            },
            
            new object[]
            {
                new Dictionary<string, object> {{"param", "s\"tr"}},
                "CYPHER param=\"s\\\"tr\" RETURN $param"
            },
            
            new object[]
            {
                new Dictionary<string, object> {{"param", new[] {1, 2, 3}}},
                "CYPHER param=[1, 2, 3] RETURN $param"
            },
            
            new object[]
            {
                new Dictionary<string, object> {{"param", new List<int> {1, 2, 3}}},
                "CYPHER param=[1, 2, 3] RETURN $param"
            },
            
            new object[]
            {
                new Dictionary<string, object> {{"param", new[] {"1", "2", "3"}}},
                "CYPHER param=[\"1\", \"2\", \"3\"] RETURN $param"
            },
            
            new object[]
            {
                new Dictionary<string, object> {{"param", new List<string> {"1", "2", "3"}}},
                "CYPHER param=[\"1\", \"2\", \"3\"] RETURN $param"
            }
        };
    }
}