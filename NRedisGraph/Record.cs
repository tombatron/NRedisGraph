using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRedisGraph
{
    public sealed class Record
    {
        public List<string> Keys { get; }

        public List<object> Values { get; }

        internal Record(List<string> header, List<object> values)
        {
            Keys = header;
            Values = values;
        }

        public T GetValue<T>(int index) => (T)Values[index];

        public T GetValue<T>(string key) => (T)Values[Keys.IndexOf(key)];

        public string GetString(int index) => Values[index].ToString();

        public string GetString(string key) => Values[Keys.IndexOf(key)].ToString();

        public bool ContainsKey(string key) => Keys.Contains(key);

        public int Size => Keys.Count;

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (!(obj is Record that))
            {
                return false;
            }

            return Enumerable.SequenceEqual(Keys, that.Keys) && Enumerable.SequenceEqual(Values, that.Values);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 31 + Keys.GetHashCode();
                hash = hash * 31 + Values.GetHashCode();

                return hash;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("Record{values=");
            sb.Append(string.Join(",", Values));
            sb.Append('}');

            return sb.ToString();
        }
    }
}