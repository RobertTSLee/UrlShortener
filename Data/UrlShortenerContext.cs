using Microsoft.EntityFrameworkCore;
using UrlShortener.Model;

namespace UrlShortener.Data;

/// <summary>
/// Database context for the URL Shortener application.
/// </summary>
/// <remarks>
/// Manages connections to the database and configures entity relationships,
/// constraints, and other model configurations using the Fluent API.
/// </remarks>
public class UrlShortenerContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the UrlShortenerContext class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public UrlShortenerContext(DbContextOptions<UrlShortenerContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the collection of Url mappings.
    /// </summary>
    public DbSet<Url> Urls { get; set; }

    /// <summary>
    /// Gets or sets the collection of Users.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Gets or sets the collection of User-Url mappings (join table).
    /// </summary>
    public DbSet<UserUrlMapping> UserUrlMappings { get; set; }

    /// <summary>
    /// Configures the model and relationships using the Fluent API.
    /// </summary>
    /// <param name="modelBuilder">The model builder for entity configuration.</param>
    /// <remarks>
    /// This method sets up:
    /// - Unique constraints on ShortUrl in Url entity
    /// - Unique constraints on UserName and Email in User entity
    /// - Foreign key relationships for UserUrlMapping
    /// - Default values and column specifications
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Url entity
        modelBuilder.Entity<Url>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<Url>()
            .HasIndex(u => u.ShortUrl)
            .IsUnique();

        modelBuilder.Entity<Url>()
            .Property(u => u.CreatedDate)
            .HasDefaultValue(DateTime.UtcNow);

        modelBuilder.Entity<Url>()
            .Property(u => u.HitCount)
            .HasDefaultValue(0);

        // Configure User entity
        modelBuilder.Entity<User>()
            .HasKey(u => u.UserId);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.CreatedDate)
            .HasDefaultValue(DateTime.UtcNow);

        // Configure UserUrlMapping entity
        modelBuilder.Entity<UserUrlMapping>()
            .HasKey(m => m.Id);

        modelBuilder.Entity<UserUrlMapping>()
            .Property(m => m.CreatedDate)
            .HasDefaultValue(DateTime.UtcNow);

        // Configure relationships
        modelBuilder.Entity<UserUrlMapping>()
            .HasOne(m => m.User)
            .WithMany(u => u.UrlMappings)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserUrlMapping>()
            .HasOne(m => m.Url)
            .WithMany(u => u.UserMappings)
            .HasForeignKey(m => m.UrlId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
