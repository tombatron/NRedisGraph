using System.Text;

namespace NRedisGraph
{
    public class Property
    {
        public string Name { get; set; }

        public ResultSet.ResultSetScalarType Type { get; set; }

        public object Value { get; set; }

        public Property(string name, ResultSet.ResultSetScalarType type, object value)
        {
            Name = name;
            Type = type;
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

            return Name == that.Name && Type == that.Type && Value == that.Value;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 31 + Name.GetHashCode();
                hash = hash * 31 + Type.GetHashCode();
                hash = hash * 31 + Value.GetHashCode();

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
            stringResult.Append(", type=");
            stringResult.Append(Type);
            stringResult.Append(", value=");
            stringResult.Append(Value);
            stringResult.Append('}');

            return stringResult.ToString();
        }
    }
}