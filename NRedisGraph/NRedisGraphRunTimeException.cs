using System;

namespace NRedisGraph
{
    [Serializable]
    public class NRedisGraphRunTimeException : Exception
    {
        public NRedisGraphRunTimeException(string message) : base(message) { }

        public NRedisGraphRunTimeException(string message, Exception inner) : base(message, inner) { }
    }
}