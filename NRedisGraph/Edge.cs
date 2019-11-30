using System.Text;

namespace NRedisGraph
{
    public class Edge : GraphEntity
    {
        public string RelationshipType { get; set; }

        public int Source { get; set; }

        public int Destination { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return false;
            }

            if (!(obj is Edge that))
            {
                return false;
            }

            if (!base.Equals(obj))
            {
                return false;
            }

            return Source == that.Source && Destination == that.Destination && RelationshipType == that.RelationshipType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 31 + base.GetHashCode();
                hash = hash * 31 + RelationshipType.GetHashCode();
                hash = hash * 31 + Source.GetHashCode();
                hash = hash * 31 + Destination.GetHashCode();

                return hash;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            return sb.ToString();
        }
    }
}