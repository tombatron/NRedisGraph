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

            var nodeB = new Node();
            nodeB.Id = 100;
            nodeB.AddLabel("L1");
            nodeB.AddProperty("Hello", "World");

            Assert.Equal(nodeA.GetHashCode(), nodeB.GetHashCode());
        }        
    }
}