using System.Collections.Generic;
using Xunit;

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
                new Dictionary<string, object> {{"param", 2.42m}},
                "CYPHER param=2.42 RETURN $param"
            },


            new object[]
            {
                new Dictionary<string, object> {{"param", 2.2f}},
                "CYPHER param=2.2 RETURN $param"
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
                new Dictionary<string, object> {{"param", new List<decimal> {1, 2.2m, 3.3m}}},
                "CYPHER param=[1, 2.2, 3.3] RETURN $param"
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
            },

            new object[]
            {
                new Dictionary<string, object> {{"param", new List<MapObject>
                {
                    new MapObject { IntProp = 1, StringProp = "my value", DecimalProp = 1 },
                    new MapObject { IntProp = 2, StringProp = "my other value", DecimalProp = 2.2m },
                    new MapObject { IntProp = 3, StringProp = "another value", DecimalProp = 3.3m },
                }}},
                "CYPHER param=[{\"IntProp\":1,\"StringProp\":\"my value\",\"DecimalProp\":1,\"ListProp\":null,\"MapProp\":null}, {\"IntProp\":2,\"StringProp\":\"my other value\",\"DecimalProp\":2.2,\"ListProp\":null,\"MapProp\":null}, {\"IntProp\":3,\"StringProp\":\"another value\",\"DecimalProp\":3.3,\"ListProp\":null,\"MapProp\":null}] RETURN $param"
            },

            new object[]
            {
                new Dictionary<string, object> {{"param", new List<MapObject>
                {
                    new MapObject
                    {
                        IntProp = 1,
                        StringProp = "my value",
                        DecimalProp = 1,
                        ListProp = new List<int> { 1, 2, 3 },
                        MapProp = new MapObject
                        {
                            IntProp = 2,
                            StringProp = "my other value",
                            DecimalProp = 2.2m,
                            ListProp = new List<int> { 4, 5, 6 },
                        }
                    },
                }}},
                "CYPHER param=[{\"IntProp\":1,\"StringProp\":\"my value\",\"DecimalProp\":1,\"ListProp\":[1,2,3],\"MapProp\":{\"IntProp\":2,\"StringProp\":\"my other value\",\"DecimalProp\":2.2,\"ListProp\":[4,5,6],\"MapProp\":null}}] RETURN $param"
            }
        };

        public class MapObject
        {
            public int IntProp { get; set; }
            public string StringProp { get; set; }
            public decimal DecimalProp { get; set; }
            public List<int> ListProp { get; set; }
            public MapObject MapProp { get; set; }
        }
    }
}