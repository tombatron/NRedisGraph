using CsvHelper.Configuration.Attributes;

namespace NRedisGraph.Demo.Imdb.Models
{
    public class Movie
    {
        [Index(0)]
        public string Title { get; set; }

        [Index(1)]
        public string Genre { get; set; }

        [Index(2)]
        public int Votes { get; set; }

        [Index(3)]
        public float Rating { get; set; }

        [Index(4)]
        public int Year { get; set; }

        public Node ToNode()
        {
            var n = new Node();

            n.AddLabel("movie");

            n.AddProperty("title", Title);
            n.AddProperty("genre", Genre);
            n.AddProperty("votes", Votes);
            n.AddProperty("rating", Rating);
            n.AddProperty("year", Year);

            return n;
        }
    }
}