namespace BookHub.Shared.DTOs;

public record PagedResult<T>(IEnumerable<T> Items, int TotalCount);