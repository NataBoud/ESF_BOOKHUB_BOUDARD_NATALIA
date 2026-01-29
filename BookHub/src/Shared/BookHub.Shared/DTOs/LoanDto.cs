namespace BookHub.Shared.DTOs;

public record LoanDto(
    Guid Id,
    Guid UserId,
    Guid BookId,
    string BookTitle,
    string UserEmail,
    DateTime LoanDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    LoanStatus Status,
    decimal PenaltyAmount
);

public record CreateLoanDto(
    Guid UserId,
    Guid BookId
);

public record ReturnLoanDto(
    DateTime ReturnDate
);

public enum LoanStatus
{
    Active,
    Returned,
    Overdue
}
