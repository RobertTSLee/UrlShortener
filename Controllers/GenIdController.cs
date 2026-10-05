using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace UrlShortener.Controllers;

[ApiController]
[Route("[controller]")]
public class GenIdController(IConnectionMultiplexer redis) : ControllerBase
{
    [HttpGet("generate-id")]
    public async Task<IActionResult> GenUniqueId()
    {
        var db = redis.GetDatabase();
        // Atomically increment counter key in Redis
        long uniqueId = await db.StringIncrementAsync("global:url:counter");

        return Ok(uniqueId);
    }
}
