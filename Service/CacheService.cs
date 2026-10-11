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
    // The exact threshold where Base62 becomes 6 characters (62^5)
    private const long SixCharStartingId = 916132832L;

    public async Task<long> GetIdAsync()
    {
        var db = redis.GetDatabase();
        var key = config.GetValue<string>(redisKey);
        var rawId = await db.StringIncrementAsync(key);       

        // add 6 char threshold offset
        return rawId > SixCharStartingId ? rawId : rawId + SixCharStartingId;
    }

}
