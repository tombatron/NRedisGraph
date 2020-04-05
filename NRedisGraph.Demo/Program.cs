using NRedisGraph.Demo.Imdb;
using NRedisGraph.Demo.Social;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace NRedisGraph.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var muxr = ConnectionMultiplexer.Connect("localhost");
            var db = muxr.GetDatabase(0);
            var redisGraph = new RedisGraph(db);

            await DoSocialAsync(redisGraph);

            Console.WriteLine("Done!");
        }

        private static async Task DoSocialAsync(RedisGraph redisGraph)
        {
            var socialUtil = new SocialUtil(redisGraph);

            await socialUtil.PopulateGraphAsync();
        }

        private static async Task DoImdbAsync(RedisGraph redisGraph)
        {
            var imdbUtil = new ImdbUtils(redisGraph);

            await imdbUtil.PopulateGraphAsync();

            foreach (var query in new ImdbQueries())
            {
                Console.WriteLine(query.Description + "\n");

                Console.WriteLine("Query:");
                Console.WriteLine("=============================================");
                Console.WriteLine(query.Query);

                Console.WriteLine("");

                var result = await redisGraph.QueryAsync("imdb", query.Query);

                Console.WriteLine("Result");
                Console.WriteLine("=============================================");
                Console.WriteLine(result.ToString());
                Console.WriteLine("*********************************************\n");
            }
        }
    }
}
