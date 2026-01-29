using BookHub.CatalogService.Domain.Entities;
using BookHub.CatalogService.Domain.Ports;
using BookHub.Shared.DTOs;

namespace BookHub.CatalogService.Application.Services;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllBooksAsync(CancellationToken cancellationToken = default);
    Task<BookDto?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<BookDto> CreateBookAsync(CreateBookDto dto, CancellationToken cancellationToken = default);
    Task<BookDto?> UpdateBookAsync(Guid id, UpdateBookDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteBookAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DecrementAvailabilityAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IncrementAvailabilityAsync(Guid id, CancellationToken cancellationToken = default);
}

public class BookService : IBookService
{
    private readonly IBookRepository _repository;
    private readonly ILogger<BookService> _logger;

    public BookService(IBookRepository repository, ILogger<BookService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksAsync(CancellationToken cancellationToken = default)
    {
        var books = await _repository.GetAllAsync(cancellationToken);
        return books.Select(MapToDto);
    }

    public async Task<BookDto?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _repository.GetByIdAsync(id, cancellationToken);
        return book == null ? null : MapToDto(book);
    }

    public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var books = await _repository.SearchAsync(searchTerm, cancellationToken);
        return books.Select(MapToDto);
    }

    public async Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        var books = await _repository.GetByCategoryAsync(category, cancellationToken);
        return books.Select(MapToDto);
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto dto, CancellationToken cancellationToken = default)
    {
        var book = Book.Create(
            dto.Title,
            dto.Author,
            dto.ISBN,
            dto.Description,
            dto.Category,
            dto.PublicationYear,
            dto.TotalCopies,
            dto.CoverImageUrl
        );

        var created = await _repository.AddAsync(book, cancellationToken);
        _logger.LogInformation("Book created: {BookId} - {Title}", created.Id, created.Title);
        return MapToDto(created);
    }

    public async Task<BookDto?> UpdateBookAsync(Guid id, UpdateBookDto dto, CancellationToken cancellationToken = default)
    {
        var book = await _repository.GetByIdAsync(id, cancellationToken);
        if (book == null) return null;

        book.Update(dto.Title, dto.Author, dto.Description, dto.Category, dto.CoverImageUrl, dto.TotalCopies);

        var updated = await _repository.UpdateAsync(book, cancellationToken);
        _logger.LogInformation("Book updated: {BookId}", updated.Id);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteBookAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.DeleteAsync(id, cancellationToken);
        if (result)
        {
            _logger.LogInformation("Book deleted: {BookId}", id);
        }
        return result;
    }

    public async Task<bool> DecrementAvailabilityAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _repository.GetByIdAsync(id, cancellationToken);
        if (book == null) return false;

        if (!book.DecrementAvailability()) return false;

        await _repository.UpdateAsync(book, cancellationToken);
        return true;
    }

    public async Task<bool> IncrementAvailabilityAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _repository.GetByIdAsync(id, cancellationToken);
        if (book == null) return false;

        if (!book.IncrementAvailability()) return false;

        await _repository.UpdateAsync(book, cancellationToken);
        return true;
    }

    private static BookDto MapToDto(Book book) => new(
        book.Id,
        book.Title,
        book.Author,
        book.ISBN,
        book.Description,
        book.Category,
        book.PublicationYear,
        book.CoverImageUrl ?? string.Empty,
        book.IsAvailable,
        book.TotalCopies,
        book.AvailableCopies
    );
}
