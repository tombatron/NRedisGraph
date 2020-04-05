using CsvHelper.Configuration.Attributes;

namespace NRedisGraph.Demo.Social.Models
{
    public class Country
    {
        [Index(0)]
        public string Name { get; set; }
    }
}