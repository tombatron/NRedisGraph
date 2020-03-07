using System.Collections.Generic;
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

        // TODO: Figure out what to do about the "compile time" exceptions. SE.Redis is just throwing a RedisServerException
        //       which could be anything I suppose...

        // [Fact]
        // public void TestSyntaxErrorReporting()
        // {
        //     // Issue a query that causes a compile-time error
        //     var exception = Assert.Throws<NRedisGraphCompileTimeException>(() =>
        //     {
        //         _api.Query("social", "RETURN toUpper(5)");
        //     });

        //     Assert.Contains("Type mismatch: expected String but was Integer", exception.Message);
        // }

        [Fact]
        public void TestRuntimeErrorReporting()
        {
            var result = _api.Query("social", "MATCH (p:person) RETURN toUpper(p.mixed_prop)");
            // Issue a query that causes a run-time error
            var exception = Assert.Throws<NRedisGraphRunTimeException>(() =>
            {
                _api.Query("social", "MATCH (p:person) RETURN toUpper(p.mixed_prop)");
            });

            Assert.Contains("Type mismatch: expected String but was Integer", exception.Message);
        }

        // [Fact]
        // public void TestExceptionFlow()
        // {
        //     var compileTimeException = Assert.Throws<NRedisGraphCompileTimeException>(() =>
        //     {
        //         _api.Query("social", "RETURN toUpper(5)");
        //     });

        //     Assert.Contains("Type mismatch: expected String but was Integer", compileTimeException.Message);

        //     var runTimeException = Assert.Throws<NRedisGraphRunTimeException>(() =>
        //     {
        //         _api.Query("social", "MATCH (p:person) RETURN toUpper(p.mixed_prop)");
        //     });

        //     Assert.Contains("Type mismatch: expected String but was Integer", runTimeException.Message);
        // }

        // [Fact]
        // public void TestMissingParametersSyntaxErrorReporting()
        // {
        //     var exception = Assert.Throws<NRedisGraphCompileTimeException>(() =>
        //     {
        //         _api.Query("social", "RETURN $param");
        //     });

        //     Assert.Contains("Missing parameters", exception.Message);
        // }

        // [Fact]
        // public void TestMissingParametersSyntaxErrorReporting2()
        // {
        //     var exception = Assert.Throws<NRedisGraphCompileTimeException>(() =>
        //     {
        //         _api.Query("social", "RETURN $param", new Dictionary<string, object>());
        //     });

        //     Assert.Contains("Missing parameters", exception.Message);
        // }

    }
}