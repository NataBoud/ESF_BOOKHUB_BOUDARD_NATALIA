namespace BookHub.Shared.DTOs;

public record BookDto(
    Guid Id,
    string Title,
    string Author,
    string ISBN,
    string Description,
    string Category,
    int PublicationYear,
    string CoverImageUrl,
    bool IsAvailable,
    int TotalCopies,
    int AvailableCopies
);

public record CreateBookDto(
    string Title,
    string Author,
    string ISBN,
    string Description,
    string Category,
    int PublicationYear,
    string? CoverImageUrl,
    int TotalCopies
);

public record UpdateBookDto(
    string? Title,
    string? Author,
    string? Description,
    string? Category,
    string? CoverImageUrl,
    int? TotalCopies
);
