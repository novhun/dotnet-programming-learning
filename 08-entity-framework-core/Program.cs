using EfCoreMastery.Data;
using EfCoreMastery.Models;
using Microsoft.EntityFrameworkCore;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("=========================================================");
Console.WriteLine("   PART 08: ENTITY FRAMEWORK CORE (EF CORE) MASTERY");
Console.WriteLine("=========================================================\n");
Console.ResetColor();

using var db = new AppDbContext();

// Ensure clean slate for each run
Console.WriteLine("[Database] Initializing SQLite database schema...");
await db.Database.EnsureDeletedAsync();
await db.Database.EnsureCreatedAsync();

// ----------------------------------------------------------------------------
// 1. SEED DATA WITH RELATIONSHIPS (1-to-N and N-to-N)
// ----------------------------------------------------------------------------
PrintHeader("1. Inserting Entities with 1-to-N and N-to-N Relationships");

var csharpTag = new Tag { Name = "C#" };
var dotnetTag = new Tag { Name = ".NET" };
var archTag = new Tag { Name = "Architecture" };

var author1 = new Author
{
    Name = "Andrew Troelsen",
    Email = "andrew.t@example.com",
    Books =
    [
        new Book
        {
            Title = "Pro C# 13 and .NET 10",
            Isbn = "978-1-4842-0001",
            Price = 54.99m,
            PublishedDate = new DateTime(2026, 1, 15),
            Tags = [csharpTag, dotnetTag]
        }
    ]
};

var author2 = new Author
{
    Name = "Martin Fowler",
    Email = "martin.f@example.com",
    Books =
    [
        new Book
        {
            Title = "Patterns of Enterprise Application Architecture",
            Isbn = "978-0-3211-2742",
            Price = 49.95m,
            PublishedDate = new DateTime(2002, 11, 5),
            Tags = [archTag]
        },
        new Book
        {
            Title = "Refactoring: Improving Code Design",
            Isbn = "978-0-1347-5759",
            Price = 45.00m,
            PublishedDate = new DateTime(2018, 11, 20),
            Tags = [archTag, csharpTag]
        }
    ]
};

await db.Authors.AddRangeAsync(author1, author2);
await db.SaveChangesAsync();
Console.WriteLine("[Database] Authors, Books, and Tags saved successfully.");

// ----------------------------------------------------------------------------
// 2. EAGER LOADING (Preventing N+1 Queries)
// ----------------------------------------------------------------------------
PrintHeader("2. Eager Loading with .Include() and .ThenInclude()");

var loadedBooks = await db.Books
    .Include(b => b.Author)
    .Include(b => b.Tags)
    .ToListAsync();

foreach (var book in loadedBooks)
{
    string tags = string.Join(", ", book.Tags.Select(t => t.Name));
    Console.WriteLine($"  📖 \"{book.Title}\" ({book.Price:C})");
    Console.WriteLine($"     Author: {book.Author?.Name} | Tags: [{tags}]");
}

// ----------------------------------------------------------------------------
// 3. PROJECTION QUERIES (High Performance DTO Selection)
// ----------------------------------------------------------------------------
PrintHeader("3. High Performance Projection (Selecting only required columns)");

var bookSummaries = await db.Books
    .AsNoTracking() // Disable tracking for read-only query
    .Where(b => b.Price > 46.00m)
    .Select(b => new
    {
        b.Title,
        b.Price,
        AuthorName = b.Author!.Name,
        TagCount = b.Tags.Count
    })
    .ToListAsync();

foreach (var summary in bookSummaries)
{
    Console.WriteLine($"  [Summary DTO] \"{summary.Title}\" by {summary.AuthorName} - {summary.Price:C} ({summary.TagCount} tags)");
}

// ----------------------------------------------------------------------------
// 4. CHANGE TRACKER & ENTITY UPDATES
// ----------------------------------------------------------------------------
PrintHeader("4. EF Core Change Tracker in Action");

var bookToUpdate = await db.Books.FirstAsync(b => b.Isbn == "978-1-4842-0001");
Console.WriteLine($"[Change Tracker] Initial state of '{bookToUpdate.Title}': {db.Entry(bookToUpdate).State}");

// Mutate property
bookToUpdate.Price = 59.99m;
Console.WriteLine($"[Change Tracker] State after modifying Price: {db.Entry(bookToUpdate).State}");

await db.SaveChangesAsync();
Console.WriteLine($"[Change Tracker] State after SaveChangesAsync: {db.Entry(bookToUpdate).State}");
Console.WriteLine($"[Database Updated] New Price: {bookToUpdate.Price:C}");

// ----------------------------------------------------------------------------
// 5. CASCADE DELETION
// ----------------------------------------------------------------------------
PrintHeader("5. Entity Deletion and Cascade Behavior");

var authorToDelete = await db.Authors.Include(a => a.Books).FirstAsync(a => a.Email == "martin.f@example.com");
Console.WriteLine($"Deleting author '{authorToDelete.Name}' with {authorToDelete.Books.Count} books...");

db.Authors.Remove(authorToDelete);
await db.SaveChangesAsync();

int remainingBooks = await db.Books.CountAsync();
Console.WriteLine($"[Cascade Delete Result] Remaining books in database: {remainingBooks} (Only 1 remains from author1)");

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n[SUCCESS] Part 08 EF Core Mastery completed successfully!\n");
Console.ResetColor();

static void PrintHeader(string title)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\n--- {title} ---");
    Console.ResetColor();
}
