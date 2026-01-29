using BookHub.CatalogService.Domain.Entities;

namespace BookHub.CatalogService.Domain.Ports;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Book> AddAsync(Book book, CancellationToken cancellationToken = default);
    Task<Book> UpdateAsync(Book book, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
