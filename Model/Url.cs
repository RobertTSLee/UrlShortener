using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.Model;

/// <summary>
/// Represents a mapping between a long URL and its shortened form.
/// </summary>
/// <remarks>
/// Unique constraints (e.g. on ShortUrl) should be configured in the DbContext using the Fluent API.
/// </remarks>
public class Url
{
    /// <summary>
    /// Primary key for the Url mapping.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The original long URL.
    /// </summary>
    [Required]
    [MaxLength(2048)]
    public string LongUrl { get; set; } = string.Empty;

    /// <summary>
    /// The shortened URL token (up to 10 characters).
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string ShortUrl { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the mapping was created (UTC).
    /// </summary>
    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Number of times the shortened URL was accessed.
    /// </summary>
    [Required]
    public int HitCount { get; set; } = 0;

    /// <summary>
    /// Navigation property for user mappings (many-to-many via UserUrlMapping).
    /// </summary>
    public ICollection<UserUrlMapping>? UserMappings { get; set; }
}
