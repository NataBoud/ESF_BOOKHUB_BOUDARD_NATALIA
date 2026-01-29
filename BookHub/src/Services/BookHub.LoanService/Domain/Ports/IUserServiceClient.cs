using BookHub.Shared.DTOs;

namespace BookHub.LoanService.Domain.Ports;

public interface IUserServiceClient
{
    Task<UserDto?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
