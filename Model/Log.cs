using System;
using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Model;

/// <summary>
/// Represents a log entry persisted in the database for audit and debugging purposes.
/// </summary>
/// <remarks>
/// Used by Serilog to store structured logs with full context including timestamps,
/// log levels, message templates, and properties for post-analysis and monitoring.
/// </remarks>
public class Log
{
    /// <summary>
    /// Primary key for the log entry.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// The timestamp when the log event was generated (UTC).
    /// </summary>
    [Required]
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The severity level of the log entry (Information, Warning, Error, Fatal, etc.).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// The log message text.
    /// </summary>
    [Required]
    public string MessageTemplate { get; set; } = string.Empty;

    /// <summary>
    /// The rendered/formatted log message with all properties interpolated.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Exception information (stack trace, type, message) if present.
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// JSON-formatted properties associated with the log entry.
    /// Contains structured data for filtering and analysis.
    /// </summary>
    public string? Properties { get; set; }

    /// <summary>
    /// The source code that generated the log entry.
    /// </summary>
    [MaxLength(500)]
    public string? SourceContext { get; set; }

    /// <summary>
    /// The request ID for correlating related logs across a single request/operation.
    /// </summary>
    [MaxLength(100)]
    public string? RequestId { get; set; }

    /// <summary>
    /// Optional user ID if the action was performed by an authenticated user.
    /// </summary>
    public int? UserId { get; set; }
}
