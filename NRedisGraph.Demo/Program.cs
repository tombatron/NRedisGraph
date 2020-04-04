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


            Console.WriteLine("Done!");
        }
    }
}
