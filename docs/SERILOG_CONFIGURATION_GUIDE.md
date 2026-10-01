## Serilog Logging Configuration Guide

### Overview

Serilog has been integrated into the UrlShortener application for structured, persistent logging. Logs are written to:
1. **Console** - For real-time monitoring during development
2. **File** - Daily rolling log files in `logs/` directory
3. **Database** - Persistent storage in PostgreSQL (Log table) via SQL Sink (optional, can be configured)

### Current Configuration (Program.cs)

```csharp
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.Enrich.FromLogContext()
	.Enrich.WithProperty("Application", "UrlShortener")
	.WriteTo.Console()
	.WriteTo.File(
		path: "logs/urlshortener-.txt",
		rollingInterval: RollingInterval.Day,
		outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
	.CreateLogger();
```

### Log Levels

| Level | Severity | Usage |
|-------|----------|-------|
| **Verbose** | Lowest | Detailed diagnostic information |
| **Debug** | Low | Debugging information useful during development |
| **Information** | Normal | General informational messages |
| **Warning** | Medium | Warning messages for potentially harmful situations |
| **Error** | High | Error messages for error events |
| **Fatal** | Highest | Fatal error that causes application shutdown |

### Database Persistence with PostgreSQL Sink

Serilog is configured to write logs directly to the PostgreSQL database using the `Serilog.Sinks.PostgreSQL` sink.

**Configuration in Program.cs:**
```csharp
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.Enrich.FromLogContext()
	.Enrich.WithProperty("Application", "UrlShortener")
	.WriteTo.Console()                    // Console output
	.WriteTo.File(...)                    // Daily rolling files
	.WriteTo.PostgreSQL(
		connectionString: pgConnectionString,
		tableName: "Logs",
		needAutoCreateTable: false)       // Use EF Core managed table
	.CreateLogger();
```

**Benefits of PostgreSQL Sink:**
- ✅ Logs written directly to database with automatic batching
- ✅ Structured log data queryable via SQL
- ✅ Better performance than EF Core write-after-facto
- ✅ Automatic timestamp and level formatting
- ✅ Rich log context and exception handling

**Note:** The `Logs` table is automatically managed by EF Core migrations. Serilog writes to this table with optimized batching for performance.

### Using Serilog in Your Code

#### Inject ILogger
```csharp
using Serilog;

public class UrlService
{
	private readonly ILogger _logger = Log.ForContext<UrlService>();

	public void ShortenUrl(string longUrl)
	{
		_logger.Information("Shortening URL: {LongUrl}", longUrl);

		try
		{
			// Processing logic
			_logger.Debug("URL processing started for {LongUrl}", longUrl);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Failed to shorten URL: {LongUrl}", longUrl);
			throw;
		}
	}
}
```

#### Using ILogger<T> in Controllers
```csharp
using Microsoft.AspNetCore.Mvc;
using Serilog;

[ApiController]
[Route("api/[controller]")]
public class UrlsController : ControllerBase
{
	private readonly ILogger<UrlsController> _logger;

	public UrlsController(ILogger<UrlsController> logger)
	{
		_logger = logger;
	}

	[HttpPost("shorten")]
	public async Task<ActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
	{
		_logger.LogInformation("POST request received for shortening URL");

		try
		{
			var result = await _shortenService.CreateShortUrl(request.LongUrl);
			_logger.LogInformation("URL shortened successfully: {ShortUrl}", result.ShortUrl);
			return Ok(result);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error shortening URL");
			return BadRequest();
		}
	}
}
```

### Structured Logging Best Practices

#### 1. Use Structured Properties
```csharp
// ❌ Bad - String concatenation
_logger.Information("User " + userId + " accessed URL " + urlId);

// ✅ Good - Structured properties
_logger.Information("User accessed URL", new { UserId = userId, UrlId = urlId });

// ✅ Even Better - Named properties
_logger.Information("User {UserId} accessed URL {UrlId}", userId, urlId);
```

#### 2. Log at Appropriate Levels
```csharp
// Information - Normal application flow
_logger.Information("User {UserId} logged in", userId);

// Debug - Detailed diagnostics (only in development)
_logger.Debug("Validating URL format for {Url}", url);

// Warning - Unexpected but recoverable situation
_logger.Warning("URL {Url} returned status {StatusCode}", url, statusCode);

// Error - Error condition that occurred
_logger.Error(ex, "Failed to save URL mapping for {UserId}", userId);

// Fatal - Critical error causing shutdown
_logger.Fatal(ex, "Database connection lost, unable to continue");
```

#### 3. Include Context
```csharp
// Enrich logs with request context
using (LogContext.PushProperty("RequestId", HttpContext.TraceIdentifier))
using (LogContext.PushProperty("UserId", currentUser.Id))
{
	_logger.Information("Processing URL shortening request");
	// All logs in this scope include RequestId and UserId
}
```

### Log Schema (Database Table)

| Column | Type | Purpose |
|--------|------|---------|
| `Id` | long | Primary key |
| `TimeStamp` | datetime | When the log occurred (UTC) |
| `Level` | nvarchar(50) | Log level (Information, Error, etc.) |
| `MessageTemplate` | text | The message template string |
| `Message` | text | Rendered message with properties |
| `Exception` | text | Exception details (stack trace, type) |
| `Properties` | text | JSON of structured properties |
| `SourceContext` | nvarchar(500) | Class/namespace that generated the log |
| `RequestId` | nvarchar(100) | HTTP request ID for correlation |
| `UserId` | int | User ID if authenticated request |

### Querying Logs from Database

```sql
-- Find all errors in the last hour
SELECT * FROM "Logs"
WHERE "Level" = 'Error'
AND "TimeStamp" > NOW() - INTERVAL '1 hour'
ORDER BY "TimeStamp" DESC;

-- Find logs for a specific request
SELECT * FROM "Logs"
WHERE "RequestId" = 'your-request-id'
ORDER BY "TimeStamp" ASC;

-- Find logs for a specific user
SELECT * FROM "Logs"
WHERE "UserId" = 123
ORDER BY "TimeStamp" DESC;

-- Count logs by level
SELECT "Level", COUNT(*) as Count
FROM "Logs"
GROUP BY "Level"
ORDER BY Count DESC;

-- Find recent exceptions
SELECT "TimeStamp", "Level", "MessageTemplate", "Exception"
FROM "Logs"
WHERE "Exception" IS NOT NULL
ORDER BY "TimeStamp" DESC
LIMIT 50;

-- Performance analysis - log volume by hour
SELECT 
    DATE_TRUNC('hour', "TimeStamp") as Hour,
    "Level",
    COUNT(*) as Count
FROM "Logs"
WHERE "TimeStamp" > NOW() - INTERVAL '24 hours'
GROUP BY DATE_TRUNC('hour', "TimeStamp"), "Level"
ORDER BY Hour DESC, Count DESC;
```

### Log File Rotation

Log files are created in the `logs/` directory with daily rotation:
- `logs/urlshortener-20261001.txt` (October 1, 2026)
- `logs/urlshortener-20261002.txt` (October 2, 2026)

**Tip:** Add `logs/` to your `.gitignore` to avoid committing log files.

### Environment-Specific Configuration

#### Development
```csharp
.MinimumLevel.Debug()
.WriteTo.Console()
.WriteTo.File(...)
```

#### Production
```csharp
.MinimumLevel.Information()
.WriteTo.PostgreSQL(connectionString, "Logs")
// Skip console output or use Application Insights
```

### Creating a Serilog Configuration from appsettings.json

For more flexibility, configure Serilog via `appsettings.json`:

**appsettings.json**
```json
{
  "Serilog": {
	"Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
	"MinimumLevel": "Information",
	"WriteTo": [
	  {
		"Name": "Console"
	  },
	  {
		"Name": "File",
		"Args": {
		  "path": "logs/urlshortener-.txt",
		  "rollingInterval": "Day"
		}
	  }
	],
	"Enrich": [ "FromLogContext", "WithProperty" ],
	"Properties": {
	  "Application": "UrlShortener"
	}
  }
}
```

**Program.cs**
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
	configuration.ReadFrom.Configuration(context.Configuration));
```

### Common Logging Scenarios

#### 1. Logging API Requests/Responses
```csharp
_logger.Information(
	"API Request: {Method} {Path} | Response: {StatusCode} | Duration: {ElapsedMs}ms",
	request.Method,
	request.Path,
	response.StatusCode,
	stopwatch.ElapsedMilliseconds);
```

#### 2. Logging Database Operations
```csharp
_logger.Debug("Executing query: {Query} with parameters {Parameters}", query, parameters);
_logger.Information("Database operation completed: {Operation} rows affected: {RowsAffected}", 
	operation, rowsAffected);
```

#### 3. Logging Business Logic Events
```csharp
_logger.Information("URL shortened successfully", new 
{ 
	LongUrl = originalUrl,
	ShortUrl = shortCode,
	UserId = user.Id,
	ExpirationDate = expiration
});
```

#### 4. Logging Exceptions
```csharp
try
{
	// Operation
}
catch (Exception ex)
{
	_logger.Error(ex, "Operation failed for {UserId} with parameters {@Parameters}",
		userId, parameters);
}
```

### Integrations (Future Enhancement)

Serilog can be extended to send logs to:
- **Azure Application Insights** - Cloud monitoring
- **Seq** - Log server for centralized analysis
- **Splunk** - Enterprise logging platform
- **ELK Stack** - Elasticsearch, Logstash, Kibana
- **Datadog** - APM and monitoring
- **New Relic** - Application performance monitoring

### Performance Considerations

- **Buffering:** Serilog batches logs for efficiency
- **Asynchronous Writing:** Use async sinks to avoid blocking requests
- **Log Level Filtering:** Only log necessary information in production
- **Batch Size:** Configure batch sizes for database writes

### Troubleshooting

**Logs not appearing in console?**
- Check MinimumLevel setting
- Verify environment (Development vs Production)

**Large log file sizes?**
- Reduce MinimumLevel (set to Information or Warning)
- Implement archival/cleanup of old logs
- Use sampling for high-volume operations

**Performance impact?**
- Use async sinks for I/O operations
- Implement buffering for batch writes
- Consider filtering to reduce log volume

---

**Last Updated:** Applied to UrlShortener project
**Serilog Version:** 4.1.0
**Sinks Included:** Console, File, PostgreSQL (configured in DbContext)
