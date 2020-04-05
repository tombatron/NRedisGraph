using CsvHelper.Configuration.Attributes;

namespace NRedisGraph.Demo.Social.Models
{
    public class Friend
    {
        [Index(0)]
        public string Person { get; set; }

        [Index(1)]
        public string FriendName { get; set; }
    }
}