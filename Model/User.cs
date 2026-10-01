using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Model;

/// <summary>
/// Represents an application user who can own shortened URLs.
/// </summary>
/// <remarks>
/// Unique constraints (e.g. on UserName and Email) should be configured in the DbContext using the Fluent API.
/// </remarks>
public class User
{
    /// <summary>
    /// Primary key for the user.
    /// </summary>
    [Key]
    public int UserId { get; set; }

    /// <summary>
    /// The user's unique username.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// The user's unique email address.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the user was created (UTC).
    /// </summary>
    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property for the user's URL mappings (many-to-many via UserUrlMapping).
    /// </summary>
    public ICollection<UserUrlMapping>? UrlMappings { get; set; }
}
