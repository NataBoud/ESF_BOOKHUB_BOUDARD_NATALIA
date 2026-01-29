using BookHub.Shared.DTOs;

namespace BookHub.BlazorClient.Services;

public interface ILoanService
{
    Task<IEnumerable<LoanDto>> GetUserLoansAsync(Guid userId);
    Task<IEnumerable<LoanDto>> GetOverdueLoansAsync();
    Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto);
    Task<LoanDto?> ReturnLoanAsync(Guid loanId);
}
