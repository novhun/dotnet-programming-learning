namespace EfCoreMastery.Data;

using EfCoreMastery.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Use self-contained SQLite file database
        optionsBuilder.UseSqlite("Data Source=library_demo.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API configuration for Author
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(150);
            entity.Property(a => a.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(a => a.Email).IsUnique();
        });

        // Fluent API configuration for Book
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title).IsRequired().HasMaxLength(250);
            entity.Property(b => b.Isbn).IsRequired().HasMaxLength(20);
            entity.Property(b => b.Price).HasPrecision(18, 2);
            entity.HasIndex(b => b.Isbn).IsUnique();

            // 1-to-Many Relationship (Author -> Books)
            entity.HasOne(b => b.Author)
                  .WithMany(a => a.Books)
                  .HasForeignKey(b => b.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Many-to-Many Relationship (Books <-> Tags)
            entity.HasMany(b => b.Tags)
                  .WithMany(t => t.Books);
        });

        // Fluent API configuration for Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(t => t.Name).IsUnique();
        });
    }
}
