using BookHub.LoanService.Domain.Entities;
using BookHub.LoanService.Domain.Ports;
using BookHub.Shared.DTOs;

namespace BookHub.LoanService.Application.Services;

public interface ILoanService
{
    Task<IEnumerable<LoanDto>> GetAllLoansAsync(CancellationToken cancellationToken = default);
    Task<LoanDto?> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);
    Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default);
    Task<LoanDto?> ReturnLoanAsync(Guid id, CancellationToken cancellationToken = default);
}

public class LoanService : ILoanService
{
    private readonly ILoanRepository _repository;
    private readonly ICatalogServiceClient _catalogClient;
    private readonly IUserServiceClient _userClient;
    private readonly ILogger<LoanService> _logger;

    public LoanService(
        ILoanRepository repository,
        ICatalogServiceClient catalogClient,
        IUserServiceClient userClient,
        ILogger<LoanService> logger)
    {
        _repository = repository;
        _catalogClient = catalogClient;
        _userClient = userClient;
        _logger = logger;
    }

    public async Task<IEnumerable<LoanDto>> GetAllLoansAsync(CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetAllAsync(cancellationToken);
        return loans.Select(MapToDto);
    }

    public async Task<LoanDto?> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        return loan == null ? null : MapToDto(loan);
    }

    public async Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return loans.Select(MapToDto);
    }

    public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<LoanDto?> ReturnLoanAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private static LoanDto MapToDto(Loan loan) => new(
        loan.Id,
        loan.UserId,
        loan.BookId,
        loan.BookTitle,
        loan.UserEmail,
        loan.LoanDate,
        loan.DueDate,
        loan.ReturnDate,
        (Shared.DTOs.LoanStatus)(int)loan.Status,
        loan.IsOverdue ? loan.CalculatePenalty() : loan.PenaltyAmount
    );
}
