namespace EfCoreMastery.Models;

public class Author
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    
    // Navigation property: One Author -> Many Books
    public List<Book> Books { get; set; } = [];
}

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Isbn { get; set; }
    public decimal Price { get; set; }
    public DateTime PublishedDate { get; set; }

    // Foreign Key
    public int AuthorId { get; set; }
    // Navigation property
    public Author? Author { get; set; }

    // Many-to-Many Navigation property: Many Books <-> Many Tags
    public List<Tag> Tags { get; set; } = [];
}

public class Tag
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public List<Book> Books { get; set; } = [];
}
