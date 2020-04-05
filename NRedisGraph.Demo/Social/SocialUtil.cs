using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace NRedisGraph.Demo.Social
{
    using System.IO; // There is a `Path` class in the NRedisGraph library. 
    using NRedisGraph.Demo.Social.Models;

    public class SocialUtil
    {
        private readonly RedisGraph _redisGraph;

        public SocialUtil(RedisGraph redisGraph)
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
            IEnumerable<Country> countries;

            using (var reader = new StreamReader(countriesFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                countries = csv.GetRecords<Country>();

                foreach (var country in countries)
                {
                    await _redisGraph.AddNodeAsync("social", country.ToNode());
                }
            }

            var personFile = Path.Combine(directory, "Resources", "person.csv");
            IEnumerable<Person> people;

            using (var reader = new StreamReader(personFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                people = csv.GetRecords<Person>();

                foreach (var person in people)
                {
                    await _redisGraph.AddNodeAsync("social", person.ToNode());
                }
            }

            var visitsFile = Path.Combine(directory, "Resources", "visits.csv");
            IEnumerable<Visit> visits;

            using (var reader = new StreamReader(visitsFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                visits = csv.GetRecords<Visit>();

                foreach (var visit in visits)
                {
                    await _redisGraph.QueryAsync("social", "MATCH (p:person{name:$person}), (c:country{name:$country}) CREATE (p)-[:visited{purpose: $purpose}]->(c)",
                        new Dictionary<string, object>
                        {
                            { "person", visit.Person },
                            { "country", visit.Country },
                            { "purpose", visit.Purpose }
                        }
                    );
                }
            }

            var friendsFile = Path.Combine(directory, "Resources", "friends.csv");
            IEnumerable<Friend> friends;

            using (var reader = new StreamReader(visitsFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                friends = csv.GetRecords<Friend>();

                foreach (var friend in friends)
                {
                    await _redisGraph.QueryAsync("social", "MATCH (p:person{name:$person}), (f:person{name:$friend}) CREATE (p)-[:friend]->(f)",
                        new Dictionary<string, object>
                        {
                            { "person", friend.Person },
                            { "friend", friend.FriendName }
                        }
                    );
                }
            }
        }
    }
}