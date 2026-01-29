using System.Net.Http.Json;
using BookHub.LoanService.Domain.Ports;
using BookHub.Shared.DTOs;

namespace BookHub.LoanService.Infrastructure.HttpClients;

public class CatalogServiceClient : ICatalogServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CatalogServiceClient> _logger;

    public CatalogServiceClient(HttpClient httpClient, ILogger<CatalogServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<BookDto?> GetBookAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BookDto>($"api/books/{bookId}", cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to get book {BookId}", bookId);
            return null;
        }
    }

    public async Task<bool> DecrementAvailabilityAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/books/{bookId}/decrement-availability", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to decrement availability for book {BookId}", bookId);
            return false;
        }
    }

    public async Task<bool> IncrementAvailabilityAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/books/{bookId}/increment-availability", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to increment availability for book {BookId}", bookId);
            return false;
        }
    }
}
