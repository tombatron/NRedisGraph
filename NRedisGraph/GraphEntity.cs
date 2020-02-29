using System.Text;
using System.Collections.Generic;

namespace NRedisGraph
{
    public abstract class GraphEntity
    {
        public int Id { get; set; }

        public IDictionary<string, Property> PropertyMap = new Dictionary<string, Property>();

        public void AddProperty(string name, object value) =>
            AddProperty(new Property(name, value));

        public void AddProperty(Property property) => PropertyMap.Add(property.Name, property);

        public void RemoveProperty(string name) => PropertyMap.Remove(name);

        public int NumberOfProperties => PropertyMap.Count;

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (!(obj is GraphEntity that))
            {
                return false;
            }

            return Id == that.Id && PropertyMap.SequenceEqual(that.PropertyMap);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 31 + Id.GetHashCode();

                foreach(var prop in PropertyMap)
                {
                    hash = hash * 31 + prop.Key.GetHashCode();
                    hash = hash * 31 + prop.Value.GetHashCode();
                }

                return hash;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("GraphEntity{id=");
            sb.Append(Id);
            sb.Append(", propertyMap=");
            sb.Append(PropertyMap);
            sb.Append('}');

            return sb.ToString();
        }
    }
}