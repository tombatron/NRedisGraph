using System.Collections.Generic;

namespace NRedisGraph
{
    internal static class IDictionaryExtensions
    {
        internal static void PutIfAbsent<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(key, value);
            }
        }

        internal static bool SequenceEqual<TKey, TValue>(this IDictionary<TKey, TValue> @this, IDictionary<TKey, TValue> that)
        {
            return false;
        }
    }
}