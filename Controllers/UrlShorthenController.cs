using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using UrlShortener.Service;

namespace UrlShortener.Controllers;

[ApiController]
[Route("[controller]")]
public class UrlShorthenController(
    IUrlService service
) : ControllerBase
{
    /*
     * NOTE:
     * 301 Moved Permanently: 
     *  This status code indicates that the resource requested has been permanently moved to a new URL. 
     *  When a client receives this response, it should update its bookmarks and use the new URL for future requests. 
     *  Search engines will also update their indexes to reflect the new URL. (SEO rank)
     *  Client browsers will cache this response and automatically redirect future requests to the new URL. (wont hit our API again, until clear cache)
     * 302 Found (Temporary Redirect): 
     *  This status code indicates that the resource requested has been temporarily moved to a different URL. 
     *  When a client receives this response, it should continue to use the original URL for future requests. 
     *  Search engines will not update their indexes to reflect the new URL.
     *  Client browsers will not cache this response and will continue to request the original URL for future requests. (will hit our API again, good to know the hit count)
     *  
     */
    // TODO: implement cache
    [HttpGet("{shortCode}")]
    public async ValueTask<IResult> RedirectUrl(string shortCode, CancellationToken ct)
    {

        var url = await service.GetUrlAsync(shortCode);
        if (url is null)
        {
            return Results.NotFound();
        }
        return Results.Redirect(url.LongUrl, permanent: false);
    }

    [HttpPost(Name = "ShortenUrl")]
    public async ValueTask<IActionResult> ShortenUrl([FromBody] string longUrl, CancellationToken ct)
    {
        //var url = await service.ShortenUrlAsync(longUrl);

        //return Results.Redirect(url.LongUrl, permanent: false);
        var url = await service.ShortenUrlAsync(longUrl);

        return Ok(url);
    }

    //[HttpPost(Name = "ShortenUrlNoRedirect")]
    //public async ValueTask<IActionResult> ShortenUrlOffLoad([FromBody] string longUrl, CancellationToken ct)
    //{
    //    var url = await service.ShortenUrlAsync(longUrl);

    //    return Ok(url);
    //}
}
