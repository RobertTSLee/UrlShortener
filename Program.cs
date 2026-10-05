using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using UrlShortener.Data;
using UrlShortener.Helper;
using UrlShortener.Service;

// Get connection string early for Serilog PostgreSQL sink
var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
    .AddUserSecrets<Program>(optional: true);

var configuration = configBuilder.Build();
var pgConnectionString = configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configure Serilog before building the host
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "UrlShortener")
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/urlshortener-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.PostgreSQL(
        connectionString: pgConnectionString,
        tableName: "Logs",
        needAutoCreateTable: false)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog to the logging pipeline
    builder.Host.UseSerilog();

    // Configure Entity Framework Core with PostgreSQL
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContext<UrlShortenerContext>(options =>
        options.UseNpgsql(connectionString));

    // Register channel for background processing of URL ingestion
    builder.Services.AddSingleton<IUrlChannel, UrlChannel>();
    builder.Services.AddHostedService<UrlIngestor>();

    // Add services to the container.
    builder.Services.AddTransient<IGuidGenerator, GuidGenerator>();

    // example of swapping IUrlConverter implementation
    builder.Services.AddTransient<IUrlConverter, Base62GuidConverter>();
    //builder.Services.AddTransient<IUrlConverter, Base64GuidConverter>();
    //builder.Services.AddTransient<IUrlConverter, Base64UrlConverter>();  

    // Register UrlService as a scoped service, example of swapping implementation of IUrlService
    builder.Services.AddScoped<IUrlService, UrlService>();
    //builder.Services.AddScoped<IUrlService, UrlHighConcurrentService>();

    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    // Add CORS policy to allow Swagger UI to access the API
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("LocalHost", policy =>
        {
            // local test purpose
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // setup swagger and OpenAPI only in development environment
    if (app.Environment.IsDevelopment())
    {
        // Enable CORS for development
        app.UseCors("LocalHost");

        // 2. Map the native OpenAPI JSON endpoint (Generates /openapi/v1.json)
        app.MapOpenApi();

        // 3. Serve the interactive Swagger UI
        app.UseSwaggerUI(options =>
        {
            // Point to the .NET 10 native OpenAPI document route
            options.SwaggerEndpoint("/openapi/v1.json", "API v1");
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Starting UrlShortener application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
