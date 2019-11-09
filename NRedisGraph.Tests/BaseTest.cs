using System;

namespace NRedisGraph.Tests
{
    public abstract class BaseTest : IDisposable
    {
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