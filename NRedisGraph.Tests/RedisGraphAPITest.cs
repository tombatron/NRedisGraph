// .NET port of https://github.com/RedisGraph/JRedisGraph
using System;
using System.Linq;
using StackExchange.Redis;
using Xunit;
using static NRedisGraph.Statistics;

namespace NRedisGraph.Tests
{
    public class RedisGraphAPITest : BaseTest
    {
        private ConnectionMultiplexer _muxr;
        private RedisGraph _api;

        public RedisGraphAPITest() : base() { }

        protected override void BeforeTest()
        {
            _muxr = ConnectionMultiplexer.Connect("localhost");
            _api = new RedisGraph(_muxr.GetDatabase(0));
        }

        protected override void AfterTest()
        {
            _api = null;
            _muxr.Dispose();
            _muxr = null;
        }

        [Fact]
        public void TestCreateNode()
        {
            // Create a node    	
            ResultSet resultSet = _api.Query("social", "CREATE ({name:'roi',age:32})");

            Assert.Equal(1, resultSet.Statistics.NodesCreated);
            Assert.Null(resultSet.Statistics.GetStringValue(Label.NODES_DELETED));
            Assert.Null(resultSet.Statistics.GetStringValue(Label.RELATIONSHIPS_CREATED));
            Assert.Null(resultSet.Statistics.GetStringValue(Label.RELATIONSHIPS_DELETED));
            Assert.Equal(2, resultSet.Statistics.PropertiesSet);
            Assert.NotNull(resultSet.Statistics.GetStringValue(Label.QUERY_INTERNAL_EXECUTION_TIME));

            Assert.Equal(0, resultSet.Count());
        }
    }
}