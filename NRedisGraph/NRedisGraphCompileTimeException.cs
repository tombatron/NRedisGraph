using System;

namespace NRedisGraph
{
    /// <summary>
    /// RedisGraph compile time exception.
    /// 
    /// The intent here would be to throw the exception when there is an exception during the evaluation of a Cypher
    /// query against RedisGraph, but I didn't see a way to discriminate between the exceptions that are throw by
    /// StackExchange.Redis. So for now this isn't used.
    /// </summary>
    [Serializable]
    public class NRedisGraphCompileTimeException : Exception
    {
        /// <summary>
        /// Create an instance using an error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns></returns>
        public NRedisGraphCompileTimeException(string message) : base(message) { }

        /// <summary>
        /// Create an instance using an error message and an inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="inner">The inner exception.</param>
        /// <returns></returns>
        public NRedisGraphCompileTimeException(string message, Exception inner) : base(message, inner) { }
    }
}