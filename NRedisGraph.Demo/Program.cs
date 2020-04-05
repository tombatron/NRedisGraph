using NRedisGraph.Demo.Imdb;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace NRedisGraph.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var muxr = ConnectionMultiplexer.Connect("localhost");
            var db = muxr.GetDatabase(0);
            var redisGraph = new RedisGraph(db);

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


            Console.WriteLine("Done!");
        }
    }
}
