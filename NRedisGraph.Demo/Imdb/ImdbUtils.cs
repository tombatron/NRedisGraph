using CsvHelper;
using NRedisGraph.Demo.Imdb.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace NRedisGraph.Demo.Imdb
{
    using System.IO; // There is a `Path` class in the NRedisGraph library.

    public class ImdbUtils
    {
        private readonly RedisGraph _redisGraph;

        public ImdbUtils(RedisGraph redisGraph)
        {
            _redisGraph = redisGraph;
        }

        public async Task PopulateGraphAsync()
        {
            if (await _redisGraph.GraphExistsAsync("imdb"))
            {
                return;
            }

            var directory = Directory.GetCurrentDirectory();

            var moviesFile = Path.Combine(directory, "Resources", "movies.csv");
            var movies = new Dictionary<string, Movie>();

            using (var reader = new StreamReader(moviesFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var movieRecords = csv.GetRecords<Movie>();

                foreach (var movie in movieRecords)
                {
                    if (!movies.ContainsKey(movie.Title))
                    {
                        var result = await _redisGraph.AddNodeAsync("imdb", movie.ToNode());

                        movies[movie.Title] = movie;
                    }
                }
            }

            var actorFile = Path.Combine(directory, "Resources", "actors.csv");
            var actors = new Dictionary<string, Actor>();

            using (var reader = new StreamReader(actorFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var actorRecords = csv.GetRecords<Actor>();

                foreach (var actor in actorRecords)
                {
                    if (!actors.ContainsKey(actor.Name))
                    {
                        await _redisGraph.AddNodeAsync("imdb", actor.ToNode());

                        actors[actor.Name] = actor;
                    }

                    if (movies.ContainsKey(actor.Movie))
                    {
                        // await _redisGraph.QueryAsync("imdb", $"MATCH (a:actor), (b:movie) WHERE a.name = \"{actor.Name}\" AND b.title = \"{actor.Movie}\" CREATE (a)-[:act]->(b)");
                        await _redisGraph.QueryAsync("imdb", $"MATCH (a:actor), (b:movie) WHERE a.name = $actorName AND b.title = $movieTitle CREATE (a)-[:act]->(b)",
                            new Dictionary<string, object>
                            {
                                { "actorName", actor.Name }, { "movieTitle", actor.Movie }
                            }
                        );
                    }
                }
            }
        }

    }
}