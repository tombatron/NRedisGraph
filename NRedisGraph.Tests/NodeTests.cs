using Xunit;

namespace NRedisGraph.Tests
{
    public class NodeTests
    {
        [Fact]
        public void HashCodeIsDeterministic()
        {
            var nodeA = new Node();
            nodeA.Id = 100;
            nodeA.AddLabel("L1");
            nodeA.AddProperty("Hello", "World");
            nodeA.AddProperty(new Property("array", new object[] { 1, 2, 3 }));

            var nodeB = new Node();
            nodeB.Id = 100;
            nodeB.AddLabel("L1");
            nodeB.AddProperty("Hello", "World");
            nodeB.AddProperty(new Property("array", new object[] { 1, 2, 3 }));

            Assert.Equal(nodeA.GetHashCode(), nodeB.GetHashCode());
        }
    }
}