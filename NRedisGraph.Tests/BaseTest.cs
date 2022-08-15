using System;

namespace NRedisGraph.Tests
{
    public abstract class BaseTest : IDisposable
    {
        public string RedisConnectionString { get; } = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost";

        protected abstract void BeforeTest();

        protected abstract void AfterTest();

        public BaseTest()
        {
            BeforeTest();
        }

        public void Dispose()
        {
            AfterTest();
        }
    }
}