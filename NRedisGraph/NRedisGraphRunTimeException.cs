using System;

namespace NRedisGraph
{
    /// <summary>
    /// RedisGraph runtime exception. This is thrown when RedisGraph encounters a runtime error during query execution.
    /// </summary>
    [Serializable]
    public class NRedisGraphRunTimeException : Exception
    {
        /// <summary>
        /// Create an instance using just a message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns></returns>
        public NRedisGraphRunTimeException(string message) : base(message) { }

        /// <summary>
        /// Create an instance using a message and an inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="inner">The inner exception.</param>
        /// <returns></returns>
        public NRedisGraphRunTimeException(string message, Exception inner) : base(message, inner) { }
    }
}