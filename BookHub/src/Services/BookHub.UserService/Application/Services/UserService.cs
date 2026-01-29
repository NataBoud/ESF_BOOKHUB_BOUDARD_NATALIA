using BookHub.Shared.DTOs;
using BookHub.UserService.Domain.Entities;
using BookHub.UserService.Domain.Ports;

namespace BookHub.UserService.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserDto> RegisterAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<LoginResponseDto?> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository repository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<UserService> logger)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetAllAsync(cancellationToken);
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByEmailAsync(email, cancellationToken);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> RegisterAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        if (await _repository.ExistsAsync(dto.Email, cancellationToken))
        {
            throw new InvalidOperationException("A user with this email already exists");
        }

        var passwordHash = _passwordHasher.Hash(dto.Password);
        var user = User.Create(dto.Email, passwordHash, dto.FirstName, dto.LastName);

        var created = await _repository.AddAsync(user, cancellationToken);
        _logger.LogInformation("User registered: {UserId} - {Email}", created.Id, created.Email);
        return MapToDto(created);
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByEmailAsync(dto.Email, cancellationToken);

        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("Login failed: User not found or inactive - {Email}", dto.Email);
            return null;
        }

        if (!user.VerifyPassword(dto.Password, _passwordHasher.Verify))
        {
            _logger.LogWarning("Login failed: Invalid password - {Email}", dto.Email);
            return null;
        }

        user.RecordLogin();
        await _repository.UpdateAsync(user, cancellationToken);

        var (token, expiresAt) = _tokenService.GenerateToken(user);

        _logger.LogInformation("User logged in: {UserId} - {Email}", user.Id, user.Email);

        return new LoginResponseDto(token, MapToDto(user), expiresAt);
    }

    public async Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        if (user == null) return null;

        user.Update(dto.FirstName, dto.LastName, dto.IsActive);

        var updated = await _repository.UpdateAsync(user, cancellationToken);
        _logger.LogInformation("User updated: {UserId}", updated.Id);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.DeleteAsync(id, cancellationToken);
        if (result)
        {
            _logger.LogInformation("User deleted: {UserId}", id);
        }
        return result;
    }

    private static UserDto MapToDto(User user) => new(
        user.Id,
        user.Email,
        user.FirstName,
        user.LastName,
        user.Role.ToString(),
        user.CreatedAt,
        user.IsActive
    );
}
