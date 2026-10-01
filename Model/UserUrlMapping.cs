using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.Model;

/// <summary>
/// Join entity that maps users to Url entities (many-to-many relationship).
/// </summary>
public class UserUrlMapping
{
    /// <summary>
    /// Primary key for the mapping.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to the user who owns or saved the URL.
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Foreign key to the Url mapping.
    /// </summary>
    [Required]
    public int UrlId { get; set; }

    /// <summary>
    /// Date and time when the mapping was created (UTC).
    /// </summary>
    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property to the User.
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    /// <summary>
    /// Navigation property to the Url.
    /// </summary>
    [ForeignKey(nameof(UrlId))]
    public Url? Url { get; set; }
}
