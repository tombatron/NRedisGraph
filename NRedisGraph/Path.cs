using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace NRedisGraph
{
    public class Path
    {
        private readonly ReadOnlyCollection<Node> _nodes;
        private readonly ReadOnlyCollection<Edge> _edges;

        public Path(IList<Node> nodes, IList<Edge> edges)
        {
            _nodes = new ReadOnlyCollection<Node>(nodes);
            _edges = new ReadOnlyCollection<Edge>(edges);
        }

        public IEnumerable<Node> Nodes => _nodes;

        public IEnumerable<Edge> Edges => _edges;

        public int Length => _edges.Count;

        public int NodeCount => _nodes.Count;

        public Node FirstNode => _nodes[0];

        public Node LastNode => _nodes.Last();

        public Node GetNode(int index) => _nodes[index];

        public Edge GetEdge(int index) => _edges[index];

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (!(obj is Path path))
            {
                return false;
            }

            return Enumerable.SequenceEqual(Nodes, path.Nodes) && Enumerable.SequenceEqual(Edges, path.Edges);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach (var node in Nodes)
                {
                    hash = hash * 31 + node.GetHashCode();
                }

                foreach (var edge in Edges)
                {
                    hash = hash * 31 + edge.GetHashCode();
                }

                return hash;
            }
        }

        public override string ToString() 
        {
            var sb = new StringBuilder();

            sb.Append("Path{");
            sb.Append($"nodes={Nodes}");
            sb.Append($", edges={Edges}");
            sb.Append("}");

            return sb.ToString();
        }
    }
}