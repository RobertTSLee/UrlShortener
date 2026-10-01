using EFCore.BulkExtensions;
using Npgsql;
using Polly;
using Polly.Retry;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using UrlShortener.Data;
using UrlShortener.Model;

namespace UrlShortener.Service;

public class UrlIngestor : BackgroundService
{
    private readonly IUrlChannel _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<UrlIngestor> _logger;
    private readonly ResiliencePipeline _resiliencePipeline;

    // todo: move these to ENV or configuration
    private const int BatchSize = 5000; 
    private static readonly TimeSpan BatchInterval = TimeSpan.FromSeconds(3);

    public UrlIngestor(
        IUrlChannel urlChannel,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<UrlIngestor> logger
    )
    {
        _channel = urlChannel;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _resiliencePipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                // 40001: Serialization failure (deadlock)
                // 53300: Too many connections
                ShouldHandle = new PredicateBuilder().Handle<NpgsqlException>(ex => ex.ErrorCode == 40001 || ex.ErrorCode == 53300),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(200),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = args =>
                {
                    _logger.LogWarning("Transient database error detected. Retry attempt #{Attempt}. Reason: {Msg}",
                        args.AttemptNumber, args.Outcome.Exception?.Message);
                    return ValueTask.CompletedTask;
                }
            }).Build();

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var buffer = new List<Url>(BatchSize);
        var lastFlushTime = DateTime.UtcNow;

        try
        {
            var readTask = ReadFromChannelAsync(buffer, stoppingToken);
            var flushTask = FlushBufferAsync(buffer, stoppingToken);

            await Task.WhenAny(readTask, flushTask);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation($"ingestor worker is stopping: {ex.Message}");

            // Flush any remaining items in the buffer before shutting down
            if (buffer.Count > 0)
            {
                await PersistMetricsAsync(buffer, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while ingesting: {ex.Message}");
        }
    }

    private async Task ReadFromChannelAsync(List<Url> buffer, CancellationToken stoppingToken)
    {
        await foreach (var item in _channel.ReadAllAsync(stoppingToken))
        {
            buffer.Add(item);     

            // buffer is full, persist the metrics to the database
            if (buffer.Count >= BatchSize)
            {
                await PersistMetricsAsync(buffer, stoppingToken);
                buffer.Clear();
            }
        }
    }

    private async Task FlushBufferAsync(List<Url> buffer, CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(BatchInterval);

        try
        {
            // Flush the buffer at regular intervals
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                if (buffer.Count > 0)
                {
                    await PersistMetricsAsync(buffer, stoppingToken);
                    buffer.Clear();
                }
            }
        }
        finally
        {
            timer.Dispose();
        }
    }

    private async Task PersistMetricsAsync(List<Url> buffer, CancellationToken cancellationToken)
    {
        if (buffer.Count == 0)
        {
            return;
        }            

        try
        {
            using var scope = _serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UrlShortenerContext>();

            try
            {
                // Execute using the Polly strategy wrapper
                await _resiliencePipeline.ExecuteAsync(async ct =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<UrlShortenerContext>();

                    using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
                    try
                    {
                        // Safe configuration to maximize database throughput
                        var bulkConfig = new BulkConfig { BatchSize = BatchSize, CalculateStats = false };

                        await dbContext.BulkInsertOrUpdateAsync(buffer, bulkConfig, cancellationToken: ct);
                        await transaction.CommitAsync(ct);                        
                    }
                    catch
                    {
                        await transaction.RollbackAsync(ct);
                        throw; // Bubbles up to Polly to trigger a retry
                    }
                }, cancellationToken);
            }
            catch (Exception fatalEx)
            {
                // fallback on retry exhaustion 
                _logger.LogCritical(fatalEx, "batch upsert failed");                
                await FallBackPersistMetricsAsync(buffer);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error persisting telemetry metrics to the database: {ex.Message}");
            throw;
        }
    }

    // todo: handle fallback on dead letter queue or file system for failed payloads
    private Task FallBackPersistMetricsAsync(List<Url> failedPayload)
    {        
        return Task.CompletedTask;
    }
}
