using System.Linq;
using System.Threading.Tasks;
using NRedisGraph;

// ReSharper disable once CheckNamespace
namespace StackExchange.Redis
{
    public static class DatabaseExtensions
    {
        public static RedisResult LolWut(this IDatabase @this, params int[] args) =>
            @this.LolWut(CommandFlags.None, args);

        public static RedisResult LolWut(this IDatabase @this, CommandFlags flags = CommandFlags.None, params int[] args) =>
            @this.Execute(Command.LOLWUT, args.Select(x => (object) x).ToList(), flags);

        public static Task<RedisResult> LolWutAsync(this IDatabase @this, params int[] args) =>
            @this.LolWutAsync(CommandFlags.None, args);

        public static Task<RedisResult> LolWutAsync(this IDatabase @this, CommandFlags flags = CommandFlags.None,
            params int[] args) =>
            @this.ExecuteAsync(Command.LOLWUT, args.Select(x => (object) x).ToList(), flags);
    }
}