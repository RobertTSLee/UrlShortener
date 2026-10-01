URL Shortener

Problem Statements
The application should receive requests via HTTP POST.
The request should contain a long url in the body, and the application should return a short url in the response.
The shortened url should be unique and should redirect to the long url when accessed.
The mapping of long urls to short urls should be stored in a database, and the application should be able to handle multiple requests concurrently.

Technical Scope
- .NET Core 10.0
- Entity Framework Core
- PostgreSQL
- ASP.NET Core Web API
- concurrency handling with async/await 
- Bulk insert for performance optimization, with a maximum of 1000 records per insert

Load Testing
- concurrent 10000 POST requests to the URL shortening endpoint
- concurrent 100000 GET requests to the shortened URL endpoint
- The shortnening logic has to implement IDisposal or ref struct on decoupled services to ensure proper resource management and avoid memory leaks during high load scenarios.
- Use a base64 encoding scheme for generating short urls to ensure uniqueness and avoid collisions.
- Use a increamental counter from DB to generate unique short urls, and encode the counter value using base64 to create the short url, avoid conflits and ensure uniqueness.

DB load estimation
- 10000 POST requests to the URL shortening endpoint will result in 10000 new records in the database.
- 100000 GET requests to the shortened URL endpoint will result in 100000 reads from the database, but no new records will be created.
- The database should be able to handle the load of 10000 new records and 100000 reads without performance degradation.
- The database should be able to handle the load of 10000 new records and 100000 reads without performance degradation.
- The data size of per records, nvarchar(2048) for long url, nvarchar(10) for short url, datetime for created date, int for hit count, and int for user id. The total data size of 10000 records will be approximately 20MB. The total data size of 100000 records will be approximately 200MB. The database should be able to handle the load of 10000 new records and 100000 reads without performance degradation.
- Indexing on the short url column will be necessary to ensure fast lookups for redirection.
- A record should expire after a certain period of time (e.g., 30 days) to prevent the database from growing indefinitely. This can be achieved by adding a created date column and periodically deleting records older than the expiration period.
- Perioudic cleanup of expired records should be implemented to maintain database performance and storage efficiency. This can be done using a scheduled job or background service that runs at regular intervals (e.g., daily) to remove expired entries.
- Maintenance job required to handle the fragmentation caused by frequent inserts and deletes. This can be achieved by periodically rebuilding or reorganizing indexes on the database tables to ensure optimal performance.

DB relationships
- DB table storing the mapping of long urls to short urls should have the following columns:
  - Id (Primary Key, int, auto-increment)
  - LongUrl (nvarchar(2048), not null)
  - ShortUrl (nvarchar(10), not null, unique)
  - CreatedDate (datetime, not null)
  - HitCount (int, default 0)
  
- DB table of users should have the following columns:
  - UserId (Primary Key, int, auto-increment)
  - UserName (nvarchar(100), not null, unique)
  - Email (nvarchar(100), not null, unique)
  - CreatedDate (datetime, not null)

- DB table of user url mappings should have the following columns:
  - Id (Primary Key, int, auto-increment)
  - UserId (Foreign Key, int, not null)
  - UrlId (Foreign Key, int, not null)
  - CreatedDate (datetime, not null)

## Implementation Status

### ✅ Completed Features
- Entity Framework Core models created
  - Url entity (Id, LongUrl, ShortUrl, CreatedDate, HitCount)
  - User entity (UserId, UserName, Email, CreatedDate)
  - UserUrlMapping entity (Id, UserId, UrlId, CreatedDate)
- DbContext configured for PostgreSQL
  - UrlShortenerContext with DbSet for Urls, Users, UserUrlMappings
  - Fluent API configuration with unique constraints and relationships
  - Foreign key relationships with cascade delete
- Initial EF Core migration created (InitialCreate)
  - Migrations/20261001065444_InitialCreate.cs
  - All tables created with proper indexes and constraints
- appsettings.json configured with PostgreSQL connection string
- NuGet packages added:
  - Microsoft.EntityFrameworkCore (v10.0.0)
  - Npgsql.EntityFrameworkCore.PostgreSQL (v10.0.0)
  - Microsoft.EntityFrameworkCore.Tools (v10.0.0)


Test cases
- Test case 1: Verify that a long URL can be successfully shortened and that the short URL redirects to the original long URL.
- Test case 2: Verify that the application can handle concurrent requests for URL shortening and redirection without errors or performance degradation.
- Test case 3: Verify that the application correctly handles invalid input, such as malformed URLs or missing parameters, and returns appropriate error responses.
- Test case 4: Verify that the application correctly handles expired short URLs and returns appropriate error responses when accessed after the expiration period.
- Test case 5: Verify that the application correctly handles database errors, such as connection failures or constraint violations, and returns appropriate error responses.
- Test case 6: Verify that the application correctly implements the base64 encoding scheme for generating unique short URLs and avoids collisions.
- Test case 7: Verify that the application correctly implements the incremental counter from the database for generating unique short URLs and avoids conflicts.
- Test case 8: Verify that the application correctly implements batching and bulk insert for performance optimization, ensuring that a maximum of 1000 records are inserted per batch.