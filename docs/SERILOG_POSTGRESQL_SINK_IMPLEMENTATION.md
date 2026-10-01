# Serilog PostgreSQL Sink Implementation Summary

## Overview

Serilog has been fully integrated into the UrlShortener application with **PostgreSQL direct database persistence** using the `Serilog.Sinks.PostgreSQL` sink. This enables:

- ✅ Real-time logging to console for development
- ✅ Persistent file-based logging with daily rotation
- ✅ Direct database logging with optimized batching
- ✅ Queryable audit trail for compliance and analysis
- ✅ Structured logging with rich context information

## Architecture

```
Application
	↓
Serilog Configuration (Program.cs)
	├── Console Sink (Real-time during development)
	├── File Sink (logs/*.txt with daily rotation)
	└── PostgreSQL Sink (Direct DB persistence)
		  ↓
	PostgreSQL Database
		└── "Logs" Table (EF Core managed)
```

## Three-Tier Logging Strategy

### 1. Console Output
**Purpose:** Real-time monitoring during development
```
[15:14:15 INF] Application started
[15:14:20 INF] User {UserId} accessed URL {UrlId}
[15:14:25 ERR] Database connection failed
```

### 2. File-Based Logging
**Purpose:** Historical records and local debugging
```
Location: logs/urlshortener-20261001.txt
Format: {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message}
Rotation: Daily (automatic)
```

### 3. PostgreSQL Database Logging
**Purpose:** Persistent, queryable audit trail for analysis
```sql
SELECT * FROM "Logs" 
WHERE "UserId" = 123 
AND "TimeStamp" > NOW() - INTERVAL '24 hours'
ORDER BY "TimeStamp" DESC;
```

## Configuration Details

### Program.cs Setup

```csharp
// Early configuration retrieval (before host building)
var pgConnectionString = configuration.GetConnectionString("DefaultConnection");

// Serilog configuration with three sinks
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.Enrich.FromLogContext()
	.Enrich.WithProperty("Application", "UrlShortener")
	.WriteTo.Console()                              // Sink 1: Console
	.WriteTo.File(...)                              // Sink 2: Daily rolling files
	.WriteTo.PostgreSQL(
		connectionString: pgConnectionString,       // Sink 3: PostgreSQL
		tableName: "Logs",
		needAutoCreateTable: false)                 // EF Core manages the table
	.CreateLogger();
```

**Key Points:**
- Connection string retrieved early (before DbContext initialization)
- PostgreSQL sink connects to same database as application
- `needAutoCreateTable: false` - Table managed by EF Core migrations
- Automatic batching for performance optimization

### Log Table Schema

| Column | Type | Purpose |
|--------|------|---------|
| `Id` | bigint | Primary key (auto-increment) |
| `TimeStamp` | timestamp with time zone | When log occurred (UTC) |
| `Level` | character varying(50) | Log level (Information, Error, etc.) |
| `MessageTemplate` | text | Template string |
| `Message` | text | Rendered message |
| `Exception` | text | Stack trace and exception details |
| `Properties` | text | JSON-formatted properties |
| `SourceContext` | character varying(500) | Class/namespace origin |
| `RequestId` | character varying(100) | HTTP request correlation ID |
| `UserId` | integer | Authenticated user ID |

**Indexes:**
- `IX_Logs_TimeStamp` (descending) - Fast recent log retrieval
- `IX_Logs_Level` - Filter by severity
- `IX_Logs_SourceContext` - Filter by component
- `IX_Logs_RequestId` - Trace related logs
- `IX_Logs_UserId` - User audit trail

## Usage Patterns

### Pattern 1: Basic Logging in Services

```csharp
private readonly Serilog.ILogger _logger = Log.ForContext<MyService>();

public void ProcessUrl(string url, int userId)
{
	_logger.Information("Processing URL for user {UserId}: {Url}", userId, url);

	try
	{
		// Operation...
		_logger.Information("URL processed successfully");
	}
	catch (Exception ex)
	{
		_logger.Error(ex, "Failed to process URL for user {UserId}", userId);
		throw;
	}
}
```

### Pattern 2: Logging with Context Correlation

```csharp
using (LogContext.PushProperty("UserId", userId))
using (LogContext.PushProperty("RequestId", HttpContext.TraceIdentifier))
{
	_logger.Information("Processing request");
	// All logs here include UserId and RequestId automatically
}
```

### Pattern 3: Performance Tracking

```csharp
var sw = Stopwatch.StartNew();
// operation
sw.Stop();

_logger.Information(
	"Operation completed in {ElapsedMs}ms",
	sw.ElapsedMilliseconds);
```

### Pattern 4: Structured Data Logging

```csharp
_logger.Information("URL shortened successfully", new
{
	UserId = userId,
	ShortCode = shortCode,
	LongUrl = longUrl,
	CreatedAt = DateTime.UtcNow
});
```

## Querying Logs from Database

### Recent Errors
```sql
SELECT "TimeStamp", "Level", "MessageTemplate", "Exception"
FROM "Logs"
WHERE "Level" = 'Error'
ORDER BY "TimeStamp" DESC
LIMIT 50;
```

### User Activity Audit Trail
```sql
SELECT "TimeStamp", "Level", "MessageTemplate"
FROM "Logs"
WHERE "UserId" = 123
ORDER BY "TimeStamp" DESC;
```

### Performance Analysis by Hour
```sql
SELECT 
	DATE_TRUNC('hour', "TimeStamp") as Hour,
	"Level",
	COUNT(*) as LogCount
FROM "Logs"
WHERE "TimeStamp" > NOW() - INTERVAL '24 hours'
GROUP BY DATE_TRUNC('hour', "TimeStamp"), "Level"
ORDER BY Hour DESC;
```

### Request Tracing
```sql
SELECT "TimeStamp", "Level", "MessageTemplate"
FROM "Logs"
WHERE "RequestId" = 'specific-request-id'
ORDER BY "TimeStamp" ASC;
```

## EF Core Migrations

Three migrations manage the database schema:

1. **20261001065444_InitialCreate.cs**
   - Creates Urls, Users, UserUrlMappings tables

2. **20261001071101_AddLoggingTable.cs**
   - Creates Logs table with all columns and indexes

3. **20261001071416_AddPostgresqlSerilogSink.cs**
   - Optimizes schema for PostgreSQL Serilog sink compatibility

## Best Practices

### Do's ✅
- Use structured properties: `_logger.Information("User {UserId} logged in", userId)`
- Log at appropriate levels: Information, Debug, Warning, Error, Fatal
- Include correlation IDs (RequestId) for request tracing
- Track user actions for audit trails
- Include timing information for performance analysis
- Catch and log exceptions with full context

### Don'ts ❌
- Don't use string concatenation: `"User " + userId` (use properties instead)
- Don't log sensitive data (passwords, tokens)
- Don't log at wrong level (everything as Information)
- Don't ignore exceptions
- Don't create new logger instances per method

## Performance Considerations

### Batching
- PostgreSQL sink automatically batches writes
- Default batch size: 25 entries
- Reduces network round-trips and DB load

### Async Operations
- File and database writes are non-blocking
- Logs don't slow down request processing

### Log Levels
- **Development:** Debug (verbose diagnostics)
- **Production:** Information (normal business events)
- Avoid Debug in production to reduce log volume

### Table Maintenance
- Monitor "Logs" table size for disk usage
- Implement cleanup policy for old logs
- Consider archive strategy for compliance

## Integration with Application Services

### Service Example
See `Service/ExampleUrlShorteningService.cs` for complete examples:
- Constructor injection pattern
- Logging at each operation step
- Exception handling with logging
- Performance tracking
- Structured data logging

### Controller Example
Demonstrates:
- Dependency injection of logger
- Logging API request/response
- Error handling and logging
- User context tracking

## Files Modified/Created

### Modified Files
- `Program.cs` - Serilog configuration with PostgreSQL sink
- `UrlShortener.csproj` - Added Serilog packages
- `Data/UrlShortenerContext.cs` - Added Log entity and DbSet
- `README.md` - Updated feature list
- `.gitignore` - Added logs/ directory

### New Files Created
- `Model/Log.cs` - Log entity with documentation
- `Service/ExampleUrlShorteningService.cs` - Implementation examples
- `docs/SERILOG_CONFIGURATION_GUIDE.md` - Comprehensive guide
- `Migrations/*_AddLoggingTable.cs` - Log table migration
- `Migrations/*_AddPostgresqlSerilogSink.cs` - Optimization migration

### Packages Added
- `Serilog` (v4.3.0) - Core logging framework
- `Serilog.AspNetCore` (v10.0.0) - ASP.NET Core integration
- `Serilog.Sinks.Console` (v6.1.1) - Console output
- `Serilog.Sinks.File` (v7.0.0) - File-based logging
- `Serilog.Sinks.PostgreSQL` (v2.3.0) - **PostgreSQL database sink**

## Running the Application

### First Time Setup
```powershell
# Update database with migrations
dotnet ef database update

# Run the application
dotnet run
```

### Logs Location
- **Console:** Terminal output in real-time
- **Files:** `logs/urlshortener-YYYYMMDD.txt` (daily rotation)
- **Database:** `Logs` table in PostgreSQL

### Monitoring Logs

**View recent logs:**
```powershell
# Last 50 lines of current log file
Get-Content logs/urlshortener*.txt -Tail 50

# Follow log file (live tail)
Get-Content logs/urlshortener*.txt -Wait
```

**Query database logs:**
```sql
-- In pgAdmin or your PostgreSQL client
SELECT * FROM "Logs" ORDER BY "TimeStamp" DESC LIMIT 100;
```

## Next Steps

1. **Remove example file:** Delete `Service/ExampleUrlShorteningService.cs` when ready
2. **Implement actual services:** Copy logging patterns to real service implementations
3. **Configure by environment:** Use different log levels for Development/Production
4. **Add application insights:** Optional - integrate Application Insights for cloud monitoring
5. **Implement log cleanup:** Add background job to archive/delete old logs

## Troubleshooting

**Logs not appearing in database?**
- Verify connection string in User Secrets
- Ensure database migrations have been applied
- Check PostgreSQL server is running

**Performance issues?**
- Reduce MinimumLevel to Information or Warning
- Monitor "Logs" table size
- Implement archival strategy for old logs

**Disk space issues?**
- Implement log retention policy
- Archive to external storage
- Reduce log level to reduce volume

---

**Status:** ✅ COMPLETE - Serilog fully integrated with PostgreSQL sink
**Version:** 1.0
**Last Updated:** October 1, 2026
