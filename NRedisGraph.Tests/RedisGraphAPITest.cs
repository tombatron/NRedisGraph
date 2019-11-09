// .NET port of https://github.com/RedisGraph/JRedisGraph
using StackExchange.Redis;

namespace NRedisGraph.Tests
{
    public class RedisGraphAPITest : BaseTest
    {
        private ConnectionMultiplexer _muxr;
        private RedisGraph _api;

        public RedisGraphAPITest() : base()
        { }

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
    }
}