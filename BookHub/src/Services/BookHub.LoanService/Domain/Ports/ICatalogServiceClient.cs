using BookHub.Shared.DTOs;

namespace BookHub.LoanService.Domain.Ports;

public interface ICatalogServiceClient
{
    Task<BookDto?> GetBookAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<bool> DecrementAvailabilityAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<bool> IncrementAvailabilityAsync(Guid bookId, CancellationToken cancellationToken = default);
}
