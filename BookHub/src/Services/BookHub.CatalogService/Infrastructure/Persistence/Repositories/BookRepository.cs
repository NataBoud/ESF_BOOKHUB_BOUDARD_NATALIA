using BookHub.CatalogService.Domain.Entities;
using BookHub.CatalogService.Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace BookHub.CatalogService.Infrastructure.Persistence.Repositories;

public class BookRepository : IBookRepository
{
    private readonly CatalogDbContext _context;

    public BookRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Books
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Books.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        return await _context.Books
            .FirstOrDefaultAsync(b => b.ISBN == isbn, cancellationToken);
    }

    public async Task<IEnumerable<Book>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _context.Books
            .Where(b => b.Category == category)
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Book>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var term = searchTerm.ToLower();
        return await _context.Books
            .Where(b => b.Title.ToLower().Contains(term) ||
                        b.Author.ToLower().Contains(term) ||
                        b.ISBN.Contains(term))
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<Book> AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync(cancellationToken);
        return book;
    }

    public async Task<Book> UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync(cancellationToken);
        return book;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await GetByIdAsync(id, cancellationToken);
        if (book == null) return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
