/// <summary>
/// Example service demonstrating best practices for using Serilog in the UrlShortener application.
/// </summary>
/// <remarks>
/// This is a template/example file showing how to integrate Serilog logging throughout the application.
/// Copy patterns from this file to your actual service implementations.
/// </remarks>
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Context;

namespace UrlShortener.Service;

/// <summary>
/// Example service for URL shortening operations with comprehensive logging.
/// </summary>
public class ExampleUrlShorteningService
{
    private readonly Serilog.ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the ExampleUrlShorteningService class.
    /// </summary>
    /// <param name="logger">The Serilog logger instance.</param>
    public ExampleUrlShorteningService(Serilog.ILogger logger)
    {
        _logger = logger.ForContext<ExampleUrlShorteningService>();
    }

    /// <summary>
    /// Example: Shorten a URL with comprehensive logging at each step.
    /// </summary>
    /// <param name="longUrl">The long URL to shorten.</param>
    /// <param name="userId">The user ID requesting the URL shortening.</param>
    /// <returns>The shortened URL code or null if operation failed.</returns>
    /// <remarks>
    /// This method demonstrates logging patterns for:
    /// - Information level: Major operations
    /// - Debug level: Detailed diagnostics
    /// - Warning level: Unexpected but recoverable situations
    /// - Error level: Error conditions that occurred
    /// </remarks>
    public async Task<string?> ShortenUrlAsync(string longUrl, int userId)
    {
        var shortCode = GenerateShortCode();

        // Information: Start of operation
        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("RequestId", Guid.NewGuid().ToString("N")))
        {
            _logger.Information(
                "Starting URL shortening process for user {UserId}",
                userId);

            try
            {
                // Debug: Input validation step
                _logger.Debug(
                    "Validating input URL: {LongUrl} | Length: {UrlLength}",
                    longUrl,
                    longUrl.Length);

                if (!IsValidUrl(longUrl))
                {
                    // Warning: Invalid input but no system error
                    _logger.Warning(
                        "Invalid URL format provided by user {UserId}: {LongUrl}",
                        userId,
                        longUrl);
                    return null;
                }

                // Debug: Processing step
                _logger.Debug(
                    "Generating short code for URL",
                    new { UrlLength = longUrl.Length, AttemptCount = 1 });

                // Debug: Database operation
                _logger.Debug(
                    "Attempting to save URL mapping to database",
                    new { ShortCode = shortCode, LongUrl = longUrl });

                // Simulate saving to database
                var success = await SaveUrlMappingAsync(shortCode, longUrl, userId);

                if (!success)
                {
                    _logger.Error(
                        "Failed to save URL mapping to database for user {UserId}",
                        userId);
                    return null;
                }

                // Information: Successful completion
                _logger.Information(
                    "URL shortened successfully",
                    new
                    {
                        UserId = userId,
                        ShortCode = shortCode,
                        LongUrl = longUrl,
                        CreatedAt = DateTime.UtcNow
                    });

                return shortCode;
            }
            catch (Exception ex)
            {
                // Error: Unexpected exception with full context
                _logger.Error(
                    ex,
                    "Exception occurred while shortening URL for user {UserId}. URL: {LongUrl}",
                    userId,
                    longUrl);

                throw;
            }
        }
    }

    /// <summary>
    /// Example: Get statistics with performance logging.
    /// </summary>
    /// <remarks>
    /// Demonstrates performance tracking and structured data logging.
    /// </remarks>
    public async Task<Dictionary<string, int>> GetUrlStatisticsAsync()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _logger.Debug("Fetching URL statistics from database");

            // Simulate database operation
            await Task.Delay(100);

            var stats = new Dictionary<string, int>
            {
                { "TotalUrls", 1000 },
                { "ActiveUrls", 850 },
                { "ExpiredUrls", 150 }
            };

            stopwatch.Stop();

            // Information: Include performance metrics
            _logger.Information(
                "URL statistics retrieved successfully in {ElapsedMs}ms",
                stopwatch.ElapsedMilliseconds,
                new { Stats = stats, QueryTime = stopwatch.ElapsedMilliseconds });

            return stats;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.Error(
                ex,
                "Failed to retrieve URL statistics. Operation took {ElapsedMs}ms",
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }

    /// <summary>
    /// Example: Batch operation with progress logging.
    /// </summary>
    /// <remarks>
    /// Demonstrates logging for long-running batch operations.
    /// </remarks>
    public async Task<int> CleanupExpiredUrlsAsync(int maxBatchSize = 100)
    {
        _logger.Information(
            "Starting cleanup of expired URLs with batch size {BatchSize}",
            maxBatchSize);

        var totalDeleted = 0;
        var batchCount = 0;

        try
        {
            while (true)
            {
                batchCount++;
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                // Simulate fetching expired URLs
                var expiredUrls = new List<string> { /* ... */ };

                if (expiredUrls.Count == 0)
                {
                    _logger.Information(
                        "No more expired URLs found. Cleanup completed.",
                        new { TotalDeleted = totalDeleted, BatchesProcessed = batchCount });
                    break;
                }

                _logger.Debug(
                    "Processing batch {BatchNumber} with {UrlCount} URLs",
                    batchCount,
                    expiredUrls.Count);

                // Simulate deletion
                var deletedCount = await DeleteUrlsAsync(expiredUrls);
                totalDeleted += deletedCount;

                stopwatch.Stop();

                _logger.Debug(
                    "Batch {BatchNumber} completed: deleted {DeletedCount} URLs in {ElapsedMs}ms",
                    batchCount,
                    deletedCount,
                    stopwatch.ElapsedMilliseconds);

                if (deletedCount < maxBatchSize)
                {
                    break; // No more records to process
                }
            }

            return totalDeleted;
        }
        catch (Exception ex)
        {
            _logger.Error(
                ex,
                "Error during cleanup of expired URLs. Processed {BatchCount} batches, deleted {DeletedCount} URLs",
                batchCount,
                totalDeleted);

            throw;
        }
    }

    // ============ Helper Methods ============

    private string GenerateShortCode()
    {
        // TODO: Implement short code generation logic
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
    }

    private bool IsValidUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> SaveUrlMappingAsync(string shortCode, string longUrl, int userId)
    {
        // TODO: Implement database save logic
        await Task.Delay(50); // Simulate DB operation
        return true;
    }

    private async Task<int> DeleteUrlsAsync(List<string> urls)
    {
        // TODO: Implement database delete logic
        await Task.Delay(50); // Simulate DB operation
        return urls.Count;
    }
}

// ============ Usage Examples in Controllers ============

/// <summary>
/// Example Controller demonstrating Serilog usage with dependency injection.
/// </summary>
public class ExampleUrlsController : ControllerBase
{
    private readonly ILogger<ExampleUrlsController> _logger;
    private readonly ExampleUrlShorteningService _service;

    /// <summary>
    /// Initializes a new instance with logger and service.
    /// </summary>
    public ExampleUrlsController(
        ILogger<ExampleUrlsController> logger,
        ExampleUrlShorteningService service)
    {
        _logger = logger;
        _service = service;
    }

    /// <summary>
    /// Example API endpoint with logging.
    /// </summary>
    public async Task<IActionResult> ShortenUrl(string longUrl, int userId)
    {
        _logger.LogInformation(
            "POST /shorten received | User: {UserId} | URL: {LongUrl}",
            userId,
            longUrl);

        try
        {
            var shortCode = await _service.ShortenUrlAsync(longUrl, userId);

            if (shortCode == null)
            {
                _logger.LogWarning(
                    "Failed to shorten URL for user {UserId}",
                    userId);

                return new BadRequestObjectResult("Invalid URL");
            }

            _logger.LogInformation(
                "Successfully shortened URL for user {UserId}",
                userId);

            return new OkObjectResult(new { shortCode });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error in ShortenUrl endpoint for user {UserId}",
                userId);

            throw;
        }
    }
}

// ============ Logging Best Practices Summary ============

/*
 * BEST PRACTICES FOR SERILOG IN THIS APPLICATION
 * 
 * 1. STRUCTURED PROPERTIES
 *    ❌ BAD:  _logger.Information("User " + userId + " shortened URL");
 *    ✅ GOOD: _logger.Information("User {UserId} shortened URL", userId);
 * 
 * 2. LOG CONTEXT FOR CORRELATION
 *    using (LogContext.PushProperty("UserId", userId))
 *    using (LogContext.PushProperty("RequestId", requestId))
 *    {
 *        // All logs in this scope include UserId and RequestId
 *    }
 * 
 * 3. APPROPRIATE LOG LEVELS
 *    - Information: Normal application flow, important business events
 *    - Debug: Detailed diagnostics useful during development
 *    - Warning: Unexpected but recoverable situations
 *    - Error: Error conditions that were handled
 *    - Fatal: Critical errors causing application shutdown
 * 
 * 4. EXCEPTION LOGGING
 *    ✅ GOOD: _logger.Error(ex, "Operation failed for {UserId}", userId);
 *    ❌ BAD:  _logger.Information("Error: " + ex.Message);
 * 
 * 5. PERFORMANCE TRACKING
 *    var sw = Stopwatch.StartNew();
 *    // operation
 *    sw.Stop();
 *    _logger.Information("Operation completed in {ElapsedMs}ms", sw.ElapsedMilliseconds);
 * 
 * 6. MULTIPLE SINKS OUTPUT
 *    - Console: Real-time monitoring during development
 *    - File: Historical records and local debugging
 *    - PostgreSQL: Queryable audit trail, analysis, and reporting
 * 
 * 7. QUERY LOGS FROM DATABASE
 *    SELECT * FROM "Logs" WHERE "UserId" = 123 ORDER BY "TimeStamp" DESC;
 *    SELECT * FROM "Logs" WHERE "Level" = 'Error' AND "TimeStamp" > NOW() - INTERVAL '1 day';
 */
