using System;

namespace NRedisGraph
{
    [Serializable]
    public class NRedisGraphCompileTimeException : Exception
    {
        public NRedisGraphCompileTimeException(string message) : base(message) { }

        public NRedisGraphCompileTimeException(string message, Exception inner) : base(message, inner) { }
    }
}