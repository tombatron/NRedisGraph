using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace NRedisGraph.Tests
{
    public class PathTests
    {
        private Node BuildNode(int id)
        {
            Node n = new Node();
            n.Id = 0;
            return n;
        }

        private Edge BuildEdge(int id, int src, int dst)
        {
            Edge e = new Edge();
            e.Id = id;
            e.Source = src;
            e.Destination = dst;
            return e;
        }

        private List<Node> BuildNodeArray(int size) =>
            Enumerable.Range(0, size).Select(i => BuildNode(i)).ToList();

        private List<Edge> BuildEdgeArray(int size) =>
             Enumerable.Range(0, size).Select(i => BuildEdge(i, i, i + 1)).ToList();

        private Path BuildPath(int nodeCount) =>
            new Path(BuildNodeArray(nodeCount), BuildEdgeArray(nodeCount - 1));

        [Fact]
        public void TestEmptyPath()
        {
            Path path = BuildPath(0);
            Assert.Equal(0, path.Length);
            Assert.Equal(0, path.Length);

            Assert.Throws<ArgumentOutOfRangeException>(() => path.GetNode(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => path.GetEdge(0));
        }

        [Fact]
        public void TestSingleNodePath()
        {
            Path path = BuildPath(1);
            Assert.Equal(0, path.Length);
            Assert.Equal(1, path.NodeCount);
            Node n = new Node();
            n.Id = 0;
            Assert.Equal(n, path.FirstNode);
            Assert.Equal(n, path.LastNode);
            Assert.Equal(n, path.GetNode(0));
        }

        [Fact]
        public void TestRandomLengthPath()
        {
            var rand = new Random();
            int nodeCount = rand.Next(2, 100 + 1);
            Path path = BuildPath(nodeCount);
            Assert.Equal(BuildNodeArray(nodeCount), path.Nodes);
            Assert.Equal(BuildEdgeArray(nodeCount - 1), path.Edges);

            path.GetEdge(0);
        }

        [Fact]
        public void HashCodeEqualTest()
        {
            var path1 = BuildPath(1);
            var path2 = BuildPath(1);
            Assert.Equal(path1, path2);
        }
    }
}