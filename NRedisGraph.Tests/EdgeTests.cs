using Xunit;

namespace NRedisGraph.Tests
{
    public class EdgeTests
    {
        [Fact]
        public void HashCodeIsDeterministic()
        {
            var edgeA = new Edge();
            edgeA.Id = 100;
            edgeA.RelationshipType = "R1";
            edgeA.Source = 1;
            edgeA.Destination = 2;
            edgeA.AddProperty("Hello", "World");

            var edgeB = new Edge();
            edgeB.Id = 100;
            edgeB.RelationshipType = "R1";
            edgeB.Source = 1;
            edgeB.Destination = 2;
            edgeB.AddProperty("Hello", "World");

            Assert.Equal(edgeA.GetHashCode(), edgeB.GetHashCode());
        }        
    }
}