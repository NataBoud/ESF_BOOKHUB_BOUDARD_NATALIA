using BookHub.UserService.Domain.Entities;

namespace BookHub.UserService.Domain.Ports;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
