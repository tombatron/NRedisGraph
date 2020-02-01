using System.Collections.Generic;
using Xunit;

namespace NRedisGraph.Tests
{
    public class ObjectsTests
    {
        [Theory]
        [MemberData(nameof(BoxedData))]
        public void CanCompareBoxedTypes(object obj1, object obj2)
        {
            Assert.True(Objects.AreEqual(obj1, obj2));
        }

        [Theory]
        [MemberData(nameof(BadBoxedData))]
        public void CanCompareNonEqualBoxedTypes(object obj1, object obj2)
        {
            Assert.False(Objects.AreEqual(obj1, obj2));
        }

        public static IEnumerable<object[]> BoxedData => new object[][]
        {
            new object[]{ (byte)25, (byte)25 },
            new object[]{ (sbyte)100, (sbyte)100 },
            new object[]{ (short)30_000, (short)30_000 },
            new object[]{ (ushort)60_000, (ushort)60_000 },
            new object[]{ (int)75_000, (int)75_000 },
            new object[]{ 850_000u, 850_000u },        
            new object[]{ 1_000_000L, 1_000_000L },
            new object[]{ 50_000_000UL, 50_000_000UL },
            new object[]{ 34_000f, 34_000f },
            new object[]{ 56_000d, 56_000d },
            new object[]{ 89_000m, 89_000m },
            new object[]{ 't', 't'},
            new object[]{ true, true },
            new object[]{ "tom", "tom" },
            new object[]{ null, null }
        };

        public static IEnumerable<object[]> BadBoxedData => new object[][]
        {
            new object[]{ (byte)25, (byte)26 },
            new object[]{ (sbyte)100, (sbyte)101 },
            new object[]{ (short)30_000, (short)30_001 },
            new object[]{ (ushort)60_000, (ushort)60_001 },
            new object[]{ (int)75_000, (int)75_001 },
            new object[]{ 850_000u, 850_001u },        
            new object[]{ 1_000_000L, 1_000_001L },
            new object[]{ 50_000_000UL, 50_000_001UL },
            new object[]{ 34_000f, 34_001f },
            new object[]{ 56_000d, 56_001d },
            new object[]{ 89_000m, 89_001m },
            new object[]{ 't', 'u'},
            new object[]{ true, false },
            new object[]{ "tom", "hanks" },
            new object[]{ "tom", null },
            new object[]{ "1", 1 }
        };        
    }
}