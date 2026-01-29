namespace BookHub.Shared.Events;

// Événements liés aux emprunts
public record LoanCreatedEvent(
    Guid LoanId,
    Guid UserId,
    Guid BookId,
    string BookTitle,
    string UserEmail,
    DateTime LoanDate,
    DateTime DueDate
);

public record LoanReturnedEvent(
    Guid LoanId,
    Guid UserId,
    Guid BookId,
    string BookTitle,
    DateTime ReturnDate,
    bool WasOverdue,
    decimal PenaltyAmount
);

public record LoanOverdueEvent(
    Guid LoanId,
    Guid UserId,
    Guid BookId,
    string BookTitle,
    string UserEmail,
    DateTime DueDate,
    int DaysOverdue,
    decimal PenaltyAmount
);

// Événements liés aux rappels
public record LoanReminderEvent(
    Guid LoanId,
    Guid UserId,
    string UserEmail,
    string BookTitle,
    DateTime DueDate,
    int DaysUntilDue
);

// Événements liés aux livres
public record BookAvailabilityChangedEvent(
    Guid BookId,
    string Title,
    bool IsAvailable,
    int AvailableCopies
);

// Événements liés aux utilisateurs
public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName
);
