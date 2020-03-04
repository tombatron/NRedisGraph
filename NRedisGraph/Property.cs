using System.Collections;
using System.Text;

namespace NRedisGraph
{
    public class Property
    {
        public string Name { get; set; }

        public object Value { get; set; }

        internal Property()
        { }

        public Property(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (!(obj is Property that))
            {
                return false;
            }

            return Name == that.Name && Objects.AreEqual(Value, that.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 31 + Name.GetHashCode();

                if (Value is IEnumerable enumerableValue)
                {
                    foreach(var value in enumerableValue)
                    {
                        hash = hash * 31 + value.GetHashCode();
                    }
                }
                else
                {
                    hash = hash * 31 + Value.GetHashCode();
                }

                return hash;
            }
        }

        public override string ToString()
        {
            var stringResult = new StringBuilder();

            stringResult.Append("Property{");
            stringResult.Append("name='");
            stringResult.Append(Name);
            stringResult.Append('\'');
            stringResult.Append(", value=");

            if (Value == null)
            {
                stringResult.Append("null");
            }
            else
            {
                stringResult.Append(Value);
            }

            stringResult.Append('}');

            return stringResult.ToString();
        }
    }
}