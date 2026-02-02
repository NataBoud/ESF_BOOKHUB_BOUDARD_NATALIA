namespace BookHub.Shared.DTOs;

public record AdminDashboardDto(
    int TotalLoans,
    int ActiveLoans,
    int OverdueLoans,
    IEnumerable<BookDto> TopBooks
);
