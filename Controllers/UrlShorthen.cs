using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Controllers;

[ApiController]
[Route("[controller]")]
public class UrlShorthen : ControllerBase
{

    [HttpPost(Name ="ShortenUrl")]
    public IActionResult ShortenUrl([FromBody] string longUrl)
    {
        // Implementation for shortening URL
        return Ok();
    }

    [HttpGet("{shortUrl}", Name = "GetLongUrl")]
    public IActionResult GetLongUrl(string shortUrl)
    {
        // Implementation for retrieving long URL
        return Ok();
    }
}
