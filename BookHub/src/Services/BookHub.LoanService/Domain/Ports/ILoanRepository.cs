using BookHub.LoanService.Domain.Entities;
using BookHub.Shared.DTOs;

namespace BookHub.LoanService.Domain.Ports;

public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Loan>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Loan>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Loan>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);
    Task<Loan?> GetActiveByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<int> GetActiveLoansCountByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Loan> AddAsync(Loan loan, CancellationToken cancellationToken = default);
    Task<Loan> UpdateAsync(Loan loan, CancellationToken cancellationToken = default);
    Task<IEnumerable<Loan>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<int> CountAllAsync(CancellationToken cancellationToken = default);
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
    Task<int> CountOverdueAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TopBookDto>> GetTopBorrowedBooksAsync(int limit, CancellationToken cancellationToken = default);

}
