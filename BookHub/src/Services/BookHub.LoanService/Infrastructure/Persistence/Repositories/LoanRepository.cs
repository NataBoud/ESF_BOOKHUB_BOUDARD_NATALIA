using BookHub.LoanService.Domain.Entities;
using BookHub.LoanService.Domain.Ports;
using Microsoft.EntityFrameworkCore;

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
            .Where(l => l.UserId == userId && l.Status == LoanStatus.Active)
            .OrderBy(l => l.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Loans
            .Where(l => l.Status == LoanStatus.Active && l.DueDate < now)
            .OrderBy(l => l.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Loan?> GetActiveByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .FirstOrDefaultAsync(l => l.BookId == bookId && l.Status == LoanStatus.Active, cancellationToken);
    }

    public async Task<int> GetActiveLoansCountByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .CountAsync(l => l.UserId == userId && l.Status == LoanStatus.Active, cancellationToken);
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
}
