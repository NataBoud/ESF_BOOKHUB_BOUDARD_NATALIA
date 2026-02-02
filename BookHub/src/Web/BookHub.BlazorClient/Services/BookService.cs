using System.Net.Http.Json;
using BookHub.Shared.DTOs;

namespace BookHub.BlazorClient.Services;

public class BookService : IBookService
{
    private readonly HttpClient _httpClient;

    public BookService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(IEnumerable<BookDto> Items, int TotalCount)> GetPagedBooksAsync(int page = 1, int pageSize = 10)
    {
        var response = await _httpClient.GetFromJsonAsync<PagedResult<BookDto>>(
            $"api/books/paged?page={page}&pageSize={pageSize}"
        );

        return response == null
            ? (Enumerable.Empty<BookDto>(), 0)
            : (response.Items, response.TotalCount);
    }

    public async Task<BookDto?> GetBookByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BookDto>($"api/books/{id}");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm)
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<BookDto>>($"api/books/search?term={Uri.EscapeDataString(searchTerm)}");
        return response ?? Enumerable.Empty<BookDto>();
    }

    public async Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(string category)
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<BookDto>>($"api/books/category/{Uri.EscapeDataString(category)}");
        return response ?? Enumerable.Empty<BookDto>();
    }
}
