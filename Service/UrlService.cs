using Microsoft.EntityFrameworkCore;
using Npgsql.Internal;
using UrlShortener.Data;
using UrlShortener.Helper;
using UrlShortener.Model;

namespace UrlShortener.Service;

public interface IUrlService
{
    Task<Url> GetUrlAsync(string shortCode);
    Task<Url> ShortenUrlAsync(string longUrl);
    Task<bool> IsValidUrlAsync(string url);
}

// abstract class to force implementation of IUrlService methods in derived classes
public abstract class UrlServiceBase( 
    IGuidGenerator guidGenerator,
    IUrlConverter converter,
    UrlShortenerContext dbContext
) : IUrlService, IDisposable
{
    protected readonly IGuidGenerator _guidGenerator = guidGenerator;
    protected readonly IUrlConverter _converter = converter;
    protected readonly UrlShortenerContext _dbContext = dbContext;
    private bool _isDisposed;

    public abstract Task<Url> GetUrlAsync(string shortCode);
    public abstract Task<Url> ShortenUrlAsync(string longUrl);
    public virtual async Task<bool> IsValidUrlAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
            return false;
        // Optionally, you can add more checks here, e.g., checking for allowed schemes (http, https)
        return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
    }

    // force derived classes to implement their own resource disposal logic if they have managed resources
    protected abstract void DisposeManagedResources();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            // Dispose managed resources here
            DisposeManagedResources();
        }

        _isDisposed = true;
    }

    ~UrlServiceBase()
    {
        Dispose(false);
    }
}

public class UrlService( 
    IGuidGenerator guidGenerator,
    IUrlConverter converter,
    UrlShortenerContext dbContext
) : UrlServiceBase(guidGenerator, converter, dbContext)
{
    public override async Task<Url> ShortenUrlAsync(string longUrl)
    {   
        if (!await IsValidUrlAsync(longUrl))
        {
            throw new ArgumentException("Invalid URL format.");
        }

        var guid = _guidGenerator.GenerateUniqueId();
        var shortUrl = _converter.Encode(guid);
        var url = new Url
        {
            LongUrl = longUrl,
            ShortUrl = shortUrl,
            CreatedDate = DateTime.UtcNow,
            HitCount = 0
        };

        await SaveUrlAsyn(url);        

        return url;
    }

    // naive implementation of saving the URL to the database, 
    // should handle collision and uniqueness in a real-world scenario
    // mitigate with GUID generation and database constraints
    protected async Task<bool> SaveUrlAsyn(Url url)
    {
        var isExists = await _dbContext.Urls.AnyAsync(u => u.ShortUrl == url.ShortUrl);
        if (isExists)
        {
            return false;
        }

        // todo: retry mechanism in case of collision or database errors
        // can use Polly of custom retry logic to handle transient failures
        _dbContext.Urls.Add(url);
        var result = await _dbContext.SaveChangesAsync();

        return result > 0;
    }

    public override async Task<Url> GetUrlAsync(string shortCode)
    {
        var url = await _dbContext.Urls
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.ShortUrl == shortCode);

        if (url != null)
        {
            url.HitCount++;
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException("Short URL not found.");
        }

        return url;
    }

    // _dContext is registered a scoped resource,
    // by right the GC is smart enough to clean it up,
    // but we can still dispose it explicitly to free up resources sooner.
    protected override void DisposeManagedResources()
    {
        _dbContext?.Dispose();
    }

}

// high throughput, high concurrency service implementation that offloads URL persistence to a background worker via a channel
public class UrlHighConcurrentService(
    IGuidGenerator guidGenerator,
    IUrlConverter converter,
    IUrlChannel urlChannel,
    UrlShortenerContext dbContext
) : IUrlService
{
    private readonly IGuidGenerator _guidGenerator = guidGenerator;
    private readonly IUrlConverter _converter = converter;
    private readonly IUrlChannel _urlChannel = urlChannel;
    private readonly UrlShortenerContext _dbContext = dbContext;

    // todo: repeated code, can sperate the logic into another validator class
    // and inject it into the service classes to avoid code duplication
    public virtual async Task<bool> IsValidUrlAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
            return false;
        // Optionally, you can add more checks here, e.g., checking for allowed schemes (http, https)
        return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
    }

    public async Task<Url> ShortenUrlAsync(string longUrl)
    {
        if (!await IsValidUrlAsync(longUrl))
        {
            throw new ArgumentException("Invalid URL format.");
        }

        var guid = _guidGenerator.GenerateUniqueId();
        var shortUrl = _converter.Encode(guid);
        var url = new Url
        {
            LongUrl = longUrl,
            ShortUrl = shortUrl,
            CreatedDate = DateTime.UtcNow,
            HitCount = 0
        };

        // push the URL to the channel for processing by a background worker
        var success = _urlChannel.TryWrite(url);
        if (!success)
        {
            throw new InvalidOperationException("Failed to write URL to channel.");
        }

        return url;
    }

    // todo: depulicated code, can sperate the logic into another getter class
    public async Task<Url> GetUrlAsync(string shortCode)
    {
        var url = await _dbContext.Urls
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.ShortUrl == shortCode);

        if (url != null)
        {
            url.HitCount++;
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException("Short URL not found.");
        }

        return url;
    }

}


// TODO: create a new service class that implements IUrlService and uses a different strategy for generating short URLs,
// e.g., using a hash of the long URL or a sequential ID.
// implement caching for frequently accessed URLs to improve performance.