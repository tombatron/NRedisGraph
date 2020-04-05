using CsvHelper.Configuration.Attributes;

namespace NRedisGraph.Demo.Social.Models
{
    public class Person
    {
        [Index(0)]
        public string Name { get; set; }

        [Index(1)]
        public int Age { get; set; }

        [Index(2)]
        public string Gender { get; set; }

        [Index(3)]
        public string Status { get; set; }
    }
}