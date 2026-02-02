using BookHub.Shared.DTOs;

namespace BookHub.BlazorClient.Services;

public interface IBookService
{
    Task<(IEnumerable<BookDto> Items, int TotalCount)> GetPagedBooksAsync(int page = 1, int pageSize = 10);
    Task<BookDto?> GetBookByIdAsync(Guid id);
    Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm);
    Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(string category);
}
