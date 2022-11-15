namespace NRedisGraph
{
    /// <summary>
    /// Container for RedisGraph result values.
    /// </summary>
    public abstract class RecordBase
    {
        /// <summary>
        /// Get a value by index.
        /// </summary>
        /// <param name="index">The index of the value you want to get.</param>
        /// <typeparam name="T">The type of the value at the index that you want to get.</typeparam>
        /// <returns>The value at the index that you specified.</returns>
        public abstract T GetValue<T>(int index);

        /// <summary>
        /// Get a value by key name.
        /// </summary>
        /// <param name="key">The key of the value you want to get.</param>
        /// <typeparam name="T">The type of the value that corresponds to the key that you specified.</typeparam>
        /// <returns>The value that corresponds to the key that you specified.</returns>
        public abstract T GetValue<T>(string key);

        /// <summary>
        /// Gets the string representation of a value at the given index.
        /// </summary>
        /// <param name="index">The index of the value that you want to get.</param>
        /// <returns>The string value at the index that you specified.</returns>
        public abstract string GetString(int index);

        /// <summary>
        /// Gets the string representation of a value by key.
        /// </summary>
        /// <param name="key">The key of the value that you want to get.</param>
        /// <returns>The string value at the key that you specified.</returns>
        public abstract string GetString(string key);

        /// <summary>
        /// Does the key exist in the record?
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public abstract bool ContainsKey(string key);
    }
}