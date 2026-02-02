using BookHub.LoanService.Domain.Entities;
using BookHub.LoanService.Domain.Ports;
using Microsoft.EntityFrameworkCore;
using BookHub.Shared.DTOs;

// Alias pour lever l'ambiguïté
using DomainLoanStatus = BookHub.LoanService.Domain.Entities.LoanStatus;

namespace BookHub.LoanService.Infrastructure.Persistence.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly LoanDbContext _context;

    public LoanRepository(LoanDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Loan>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Loans.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Loan>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Loan>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .Where(l => l.UserId == userId && l.Status == DomainLoanStatus.Active)
            .OrderBy(l => l.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Loans
            .Where(l => l.Status == DomainLoanStatus.Active && l.DueDate < now)
            .OrderBy(l => l.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Loan?> GetActiveByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .FirstOrDefaultAsync(l => l.BookId == bookId && l.Status == DomainLoanStatus.Active, cancellationToken);
    }

    public async Task<Loan?> GetActiveLoanByUserAndBookAsync(Guid userId, Guid bookId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .FirstOrDefaultAsync(l => l.BookId == bookId && l.UserId == userId && l.Status == DomainLoanStatus.Active, cancellationToken);
    }

    public async Task<int> GetActiveLoansCountByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .CountAsync(l => l.UserId == userId && l.Status == DomainLoanStatus.Active, cancellationToken);
    }

    public async Task<Loan> AddAsync(Loan loan, CancellationToken cancellationToken = default)
    {
        _context.Loans.Add(loan);
        await _context.SaveChangesAsync(cancellationToken);
        return loan;
    }

    public async Task<Loan> UpdateAsync(Loan loan, CancellationToken cancellationToken = default)
    {
        _context.Loans.Update(loan);
        await _context.SaveChangesAsync(cancellationToken);
        return loan;
    }

    public async Task<IEnumerable<Loan>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .Where(l => l.BookId == bookId)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Loans.CountAsync(cancellationToken);
    }

    public async Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Loans.CountAsync(l => l.Status == DomainLoanStatus.Active, cancellationToken);
    }

    public async Task<int> CountOverdueAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Loans.CountAsync(
            l => l.Status == DomainLoanStatus.Active && l.DueDate < now,
            cancellationToken);
    }

    public Task<IEnumerable<(Guid BookId, int LoanCount)>> GetTopBorrowedBookIdsAsync(int limit, CancellationToken cancellationToken = default)
    {
        var topBooksInMemory = _context.Loans
            .GroupBy(l => l.BookId)
            .Select(g => new { BookId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(limit)
            .AsEnumerable() // bascule en mémoire
            .Select(x => (x.BookId, x.Count)) // crée les tuples
            .ToList(); 

        // Pas d'await ici car c'est déjà en mémoire
        return Task.FromResult<IEnumerable<(Guid BookId, int LoanCount)>>(topBooksInMemory);
    }

}
