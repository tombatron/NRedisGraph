using StackExchange.Redis;
using Xunit;

namespace NRedisGraph.Tests
{
    public class NRedisGraphErrorTest : BaseTest
    {
        private ConnectionMultiplexer _muxr;
        private RedisGraph _api;

        public NRedisGraphErrorTest() : base() { }

        protected override void BeforeTest()
        {
            _muxr = ConnectionMultiplexer.Connect("localhost");

            _api = new RedisGraph(_muxr.GetDatabase(0));

            Assert.NotNull(_api.Query("social", "CREATE (:person{mixed_prop: 'strval'}), (:person{mixed_prop: 50})"));
        }

        protected override void AfterTest()
        {
            _api.DeleteGraph("social");
        }
    }
}