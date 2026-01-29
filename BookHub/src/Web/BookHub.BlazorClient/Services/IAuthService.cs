using BookHub.Shared.DTOs;

namespace BookHub.BlazorClient.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    Task<UserDto> RegisterAsync(CreateUserDto createUserDto);
}
