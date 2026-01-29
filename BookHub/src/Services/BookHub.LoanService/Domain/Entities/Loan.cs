namespace BookHub.LoanService.Domain.Entities;

public class Loan
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid BookId { get; private set; }
    public string BookTitle { get; private set; } = string.Empty;
    public string UserEmail { get; private set; } = string.Empty;
    public DateTime LoanDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public LoanStatus Status { get; private set; } = LoanStatus.Active;
    public decimal PenaltyAmount { get; private set; } = 0;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public const int MaxLoanDurationDays = 21;
    public const int MaxActiveLoansPerUser = 5;
    public const decimal PenaltyPerDay = 0.50m;

    public bool IsOverdue => Status == LoanStatus.Active && DateTime.UtcNow > DueDate;

    public int DaysOverdue => IsOverdue
        ? (int)(DateTime.UtcNow - DueDate).TotalDays
        : 0;

    private Loan() { }

    public static Loan Create(Guid userId, Guid bookId, string bookTitle, string userEmail)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required", nameof(userId));
        if (bookId == Guid.Empty)
            throw new ArgumentException("Book ID is required", nameof(bookId));
        if (string.IsNullOrWhiteSpace(bookTitle))
            throw new ArgumentException("Book title is required", nameof(bookTitle));
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email is required", nameof(userEmail));

        return new Loan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BookId = bookId,
            BookTitle = bookTitle,
            UserEmail = userEmail,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(MaxLoanDurationDays),
            Status = LoanStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
    }

    public decimal CalculatePenalty()
    {
        if (!IsOverdue) return 0;
        return DaysOverdue * PenaltyPerDay;
    }

    public void Return()
    {
        if (Status == LoanStatus.Returned)
            throw new InvalidOperationException("Loan is already returned");

        ReturnDate = DateTime.UtcNow;
        Status = LoanStatus.Returned;
        PenaltyAmount = CalculatePenalty();
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum LoanStatus
{
    Active,
    Returned,
    Overdue
}
