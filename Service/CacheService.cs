using StackExchange.Redis;
using System.Text.Json;

namespace UrlShortener.Service;

public interface ICacheService
{
    Task<long> GetIdAsync();
}

public class CacheService(
    IConnectionMultiplexer redis,
    IConfiguration config
): ICacheService
{
    private readonly string redisKey = "Redis:UrlShortener:UniqueIdKey";
    public async Task<long> GetIdAsync()
    {
        var db = redis.GetDatabase();
        var key = config.GetValue<string>(redisKey);

        return await db.StringIncrementAsync(key);
    }

}
