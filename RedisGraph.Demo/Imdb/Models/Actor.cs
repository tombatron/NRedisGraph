using System;
using CsvHelper.Configuration.Attributes;
using NRedisGraph;

namespace RedisGraph.Demo.Imdb.Models
{
    public class Actor
    {
        [Index(0)]
        public string Name { get; set; }

        [Index(1)]
        public int YearOfBirth { get; set; }

        public int Age => DateTime.Now.Year - YearOfBirth;

        [Index(2)]
        public string Movie { get; set; }

        public Node ToNode()
        {
            var n = new Node();

            n.AddLabel("actor");
            
            n.AddProperty("name", Name);
            n.AddProperty("age", Age);
            
            return n;
        }
    }
}