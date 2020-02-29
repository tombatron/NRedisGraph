using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRedisGraph
{
    public sealed class Node : GraphEntity
    {
        private readonly List<string> _labels = new List<string>();

        public void AddLabel(string label) => _labels.Add(label);

        public void RemoveLabel(string label) => _labels.Remove(label);

        public string GetLabel(int index) => _labels[index];

        public int GetNumberOfLabels() => _labels.Count;

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (!(obj is Node that))
            {
                return false;
            }

            if (!base.Equals(obj))
            {
                return false;
            }

            return Enumerable.SequenceEqual(_labels, that._labels);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                
                foreach(var label in _labels)
                {
                    hash = hash * 31 + label.GetHashCode();
                }

                hash = hash * 31 + base.GetHashCode();

                return hash;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("Node{labels=");
            sb.Append($"[{string.Join(", ", _labels)}]");
            sb.Append(", id=");
            sb.Append(Id);
            sb.Append(", propertyMap={");
            sb.Append(string.Join(", ", PropertyMap.Select(pm => $"{pm.Key}={pm.Value.ToString()}")));
            sb.Append("}}");

            return sb.ToString();
        }
    }
}