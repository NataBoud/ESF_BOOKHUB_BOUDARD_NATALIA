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
    Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<BookDto>> GetTopBorrowedBooksAsync(int limit = 5, CancellationToken cancellationToken = default);  
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
        // Vérifier que l'utilisateur existe
        var user = await _userClient.GetUserAsync(dto.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException(
                $"Utilisateur avec ID {dto.UserId} introuvable."
            );

        // Vérifier la limite de prêts utilisateur
        int activeLoans = await _repository
            .GetActiveLoansCountByUserAsync(dto.UserId, cancellationToken);

        if (activeLoans >= 5)
            throw new InvalidOperationException(
                $"L'utilisateur ne peut pas emprunter plus de 5 livres. Actuellement empruntés : {activeLoans}"
            );

        // Vérifier que le livre existe
        var book = await _catalogClient.GetBookAsync(dto.BookId, cancellationToken);
        if (book == null)
            throw new InvalidOperationException(
                $"Livre avec ID {dto.BookId} introuvable."
            );

        // Vérifier si l'utilisateur a déjà emprunté ce livre
        var userActiveLoan = await _repository
            .GetActiveLoanByUserAndBookAsync(dto.UserId, dto.BookId, cancellationToken);

        if (userActiveLoan != null)
            throw new InvalidOperationException(
                $"Vous avez déjà emprunté le livre '{book.Title}'."
            );

        // Vérifier la disponibilité (SOURCE UNIQUE : Catalog)
        if (book.AvailableCopies <= 0)
            throw new InvalidOperationException(
                $"Le livre '{book.Title}' n'est pas disponible pour l'emprunt."
            );

        // Décrémenter la disponibilité dans le CatalogService
        var decremented = await _catalogClient
            .DecrementAvailabilityAsync(dto.BookId, cancellationToken);

        if (!decremented)
            throw new InvalidOperationException(
                "Impossible de réserver le livre pour le moment."
            );

        // Créer le prêt
        var loan = Loan.Create(dto.UserId, dto.BookId, book.Title, user.Email);
        await _repository.AddAsync(loan, cancellationToken);

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

    public async Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken cancellationToken = default)
    {
        var totalLoans = await _repository.CountAllAsync(cancellationToken);
        var activeLoans = await _repository.CountActiveAsync(cancellationToken);
        var overdueLoans = await _repository.CountOverdueAsync(cancellationToken);

        // Récupérer les top books côté repository en tant que tuples (BookId, LoanCount)
        var topBookIds = await _repository.GetTopBorrowedBookIdsAsync(5, cancellationToken);

        // Compléter les infos BookDto via le CatalogService
        var topBooks = new List<BookDto>();
        foreach (var (bookId, loanCount) in topBookIds)
        {
            var book = await _catalogClient.GetBookAsync(bookId, cancellationToken);
            if (book != null)
            {
                // Copier l'objet BookDto et ajouter le LoanCount
                topBooks.Add(book with { LoanCount = loanCount });
            }
        }

        return new AdminDashboardDto(
            TotalLoans: totalLoans,
            ActiveLoans: activeLoans,
            OverdueLoans: overdueLoans,
            TopBooks: topBooks
        );
    }

    public async Task<IEnumerable<BookDto>> GetTopBorrowedBooksAsync(int limit = 5, CancellationToken cancellationToken = default)
    {
        var topBooksIds = await _repository.GetTopBorrowedBookIdsAsync(limit, cancellationToken);

        var books = new List<BookDto>();

        foreach (var (bookId, loanCount) in topBooksIds)
        {
            var book = await _catalogClient.GetBookAsync(bookId, cancellationToken);
            if (book != null)
            {
                // On peut ajouter LoanCount dans le DTO si on veut l'afficher dans admin
                var bookWithLoanCount = book with { LoanCount = loanCount };
                books.Add(bookWithLoanCount);
            }
        }

        return books;
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
