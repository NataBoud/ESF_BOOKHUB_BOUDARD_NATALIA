using BookHub.LoanService.Domain.Entities;
using BookHub.LoanService.Domain.Ports;
using BookHub.Shared.DTOs;
using LoanStatus = BookHub.LoanService.Domain.Entities.LoanStatus;

namespace BookHub.LoanService.Application.Services;

public interface ILoanService
{
    Task<IEnumerable<LoanDto>> GetAllLoansAsync(CancellationToken cancellationToken = default);
    Task<LoanDto?> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);
    Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default);
    Task<LoanDto?> ReturnLoanAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoanDto>> GetLoansByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default);
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
        var overdueLoans = await _repository.GetOverdueLoansAsync(cancellationToken);
        return overdueLoans.Select(MapToDto);
    }

    public async Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default)
    {
        // 1 Vérifier que l'utilisateur existe via le UserService (sécurisé)
        var user = await _userClient.GetUserAsync(dto.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"Utilisateur avec ID {dto.UserId} introuvable.");

        // 2 Vérifier le nombre d'emprunts actifs de l'utilisateur
        int activeLoans = await _repository.GetActiveLoansCountByUserAsync(dto.UserId, cancellationToken);
        if (activeLoans >= 5)
            throw new InvalidOperationException($"L'utilisateur ne peut pas emprunter plus de 5 livres. Actuellement empruntés : {activeLoans}");

        // 3 Vérifier que le livre existe via le CatalogService
        var book = await _catalogClient.GetBookAsync(dto.BookId, cancellationToken);
        if (book == null)
            throw new InvalidOperationException($"Livre avec ID {dto.BookId} introuvable.");

        // 4 Vérifier la disponibilité du livre
        if (book.AvailableCopies <= 0)
            throw new InvalidOperationException($"Le livre '{book.Title}' n'est pas disponible.");

        // 5 Vérifier que le livre n'est pas déjà emprunté activement
        var activeLoan = await _repository.GetActiveByBookIdAsync(dto.BookId, cancellationToken);
        if (activeLoan != null)
            throw new InvalidOperationException($"Le livre '{book.Title}' est déjà emprunté.");

        // 6 Décrémenter la disponibilité du livre dans le catalogue
        var decremented = await _catalogClient.DecrementAvailabilityAsync(dto.BookId, cancellationToken);
        if (!decremented)
            throw new InvalidOperationException("Impossible de réserver le livre pour le moment.");

        // 7 Créer le prêt via la factory de l'entité Loan
        var loan = Loan.Create(dto.UserId, dto.BookId, book.Title, user.Email);

        // 8 Sauvegarder le prêt en base
        await _repository.AddAsync(loan, cancellationToken);

        // 9 Retourner le DTO correspondant
        return MapToDto(loan);
    }

    public async Task<LoanDto?> ReturnLoanAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        if (loan == null || loan.Status != LoanStatus.Active)
            return null;

        loan.Return();

        await _repository.UpdateAsync(loan, cancellationToken);

        // Remettre la disponibilite du livre
        await _catalogClient.IncrementAvailabilityAsync(loan.BookId, cancellationToken);

        return MapToDto(loan);
    }

    public async Task<IEnumerable<LoanDto>> GetLoansByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetByBookIdAsync(bookId, cancellationToken);
        return loans.Select(MapToDto);
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
