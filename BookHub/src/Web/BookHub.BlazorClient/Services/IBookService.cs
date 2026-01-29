using BookHub.Shared.DTOs;

namespace BookHub.BlazorClient.Services;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllBooksAsync();
    Task<BookDto?> GetBookByIdAsync(Guid id);
    Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm);
    Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(string category);
}
