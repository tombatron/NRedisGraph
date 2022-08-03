using System;
using Xunit;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace NRedisGraph.Tests
{
    public class IDatabaseExtensionsTests : BaseTest
    {
        private ConnectionMultiplexer _muxr;
        private IDatabase _db;
        protected override void BeforeTest()
        {
            _muxr = ConnectionMultiplexer.Connect(RedisConnectionString);
            _db = _muxr.GetDatabase(0);
        }

        

        protected override void AfterTest()
        {
            _muxr.Dispose();
            _muxr = null;
        }

        [Fact]
        public void CanLolWut()
        {
            var result = _db.LolWut();
            
            Assert.NotNull(result.ToString());
        }

        [Fact]
        public void CanLolWutWithParams()
        {
            var result = _db.LolWut(1, 2, 3, 4, 5);
            
            Assert.NotNull(result.ToString());
        }

        [Fact]
        public async Task CanLolWutAsync()
        {
            var result = await _db.LolWutAsync();
            
            Assert.NotNull(result.ToString());
        }

        [Fact]
        public async Task CanLolWutWithParamsAsync()
        {
            var result = await _db.LolWutAsync(1, 2, 3, 4, 5);
            
            Assert.NotNull(result.ToString());
        }
    }
}