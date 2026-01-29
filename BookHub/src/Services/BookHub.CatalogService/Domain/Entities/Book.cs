namespace BookHub.CatalogService.Domain.Entities;

public class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public string ISBN { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public int PublicationYear { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public int TotalCopies { get; private set; }
    public int AvailableCopies { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public bool IsAvailable => AvailableCopies > 0;

    private Book() { }

    public static Book Create(
        string title,
        string author,
        string isbn,
        string description,
        string category,
        int publicationYear,
        int totalCopies,
        string? coverImageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author is required", nameof(author));
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN is required", nameof(isbn));
        if (totalCopies < 0)
            throw new ArgumentException("Total copies cannot be negative", nameof(totalCopies));

        return new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            Author = author,
            ISBN = isbn,
            Description = description,
            Category = category,
            PublicationYear = publicationYear,
            TotalCopies = totalCopies,
            AvailableCopies = totalCopies,
            CoverImageUrl = coverImageUrl,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string? title, string? author, string? description, string? category, string? coverImageUrl, int? totalCopies)
    {
        if (title != null) Title = title;
        if (author != null) Author = author;
        if (description != null) Description = description;
        if (category != null) Category = category;
        if (coverImageUrl != null) CoverImageUrl = coverImageUrl;

        if (totalCopies.HasValue)
        {
            var diff = totalCopies.Value - TotalCopies;
            TotalCopies = totalCopies.Value;
            AvailableCopies = Math.Max(0, AvailableCopies + diff);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public bool DecrementAvailability()
    {
        if (AvailableCopies <= 0) return false;
        AvailableCopies--;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    public bool IncrementAvailability()
    {
        if (AvailableCopies >= TotalCopies) return false;
        AvailableCopies++;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }
}
