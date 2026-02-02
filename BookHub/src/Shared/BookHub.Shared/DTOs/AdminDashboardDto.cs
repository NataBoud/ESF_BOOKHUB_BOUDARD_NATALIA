namespace BookHub.Shared.DTOs;

public record AdminDashboardDto(
    int TotalLoans,
    int ActiveLoans,
    int OverdueLoans,
    IEnumerable<TopBookDto> TopBooks
);
public record TopBookDto(
    Guid BookId,
    string Title,
    int LoanCount
);