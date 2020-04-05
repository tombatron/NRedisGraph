using CsvHelper.Configuration.Attributes;

namespace NRedisGraph.Demo.Social.Models
{
    public class Visit
    {
        [Index(0)]
        public string Person { get; set; }

        [Index(1)]
        public string Country { get; set; }

        [Index(2)]
        public string Purpose { get; set; }
    }
}