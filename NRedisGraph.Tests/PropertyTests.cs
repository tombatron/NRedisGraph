using Xunit;

namespace NRedisGraph.Tests
{
    public class PropertyTests
    {
        [Fact]
        public void HashCodeIsDeterministic()
        {
            var propertyA = new Property();
            propertyA.Name = "Hello";
            propertyA.Value = "World";

            var propertyB = new Property();
            propertyB.Name = "Hello";
            propertyB.Value = "World";

            Assert.Equal(propertyA.GetHashCode(), propertyB.GetHashCode());
        }

        [Fact]
        public void HashCodeIsDeterministicWithEnumerableValue()
        {
            var propertyA = new Property();
            propertyA.Name = "Collection";
            propertyA.Value = new[] { 1, 2, 3 };

            var propertyB = new Property();
            propertyB.Name = "Collection";
            propertyB.Value = new[] { 1, 2, 3 };

            Assert.Equal(propertyA.GetHashCode(), propertyB.GetHashCode());
        }
    }
}