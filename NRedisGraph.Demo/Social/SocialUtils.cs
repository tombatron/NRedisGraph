using CsvHelper;
using System.Globalization;
using System.Threading.Tasks;

namespace NRedisGraph.Demo.Social
{
    using System.IO; // There is a `Path` class in the NRedisGraph library. 
    public class SocialUtils
    {
        private readonly RedisGraph _redisGraph;

        public SocialUtils(RedisGraph redisGraph)
        {
            _redisGraph = redisGraph;
        }

        public async Task PopulateGraphAsync()
        {
            if (await _redisGraph.GraphExistsAsync("social"))
            {
                return;
            }

            var directory = Directory.GetCurrentDirectory();

            var countriesFile = Path.Combine(directory, "Resources", "countries.csv");

            using (var reader = new StreamReader(countriesFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

            }

            var personFile = Path.Combine(directory, "Resources", "person.csv");

            using (var reader = new StreamReader(personFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

            }

            var visitsFile = Path.Combine(directory, "Resources", "visits.csv");

            using (var reader = new StreamReader(visitsFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

            }

            var friendsFile = Path.Combine(directory, "Resources", "friends.csv");

            using (var reader = new StreamReader(visitsFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

            }
        }
    }
}