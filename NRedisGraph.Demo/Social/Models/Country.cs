using CsvHelper.Configuration.Attributes;

namespace NRedisGraph.Demo.Social.Models
{
    public class Country
    {
        [Index(0)]
        public string Name { get; set; }

        public Node ToNode()
        {
            var n = new Node();

            n.AddLabel("country");

            n.AddProperty("name", Name);

            return n;
        }
    }
}