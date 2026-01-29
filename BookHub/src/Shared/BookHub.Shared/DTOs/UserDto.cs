namespace BookHub.Shared.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    DateTime CreatedAt,
    bool IsActive
);

public record CreateUserDto(
    string Email,
    string Password,
    string FirstName,
    string LastName
);

public record UpdateUserDto(
    string? FirstName,
    string? LastName,
    bool? IsActive
);

public record LoginDto(
    string Email,
    string Password
);

public record LoginResponseDto(
    string Token,
    UserDto User,
    DateTime ExpiresAt
);
