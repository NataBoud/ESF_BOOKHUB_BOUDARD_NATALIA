namespace BookHub.UserService.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; } = UserRole.Member;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    private User() { }

    public static User Create(string email, string passwordHash, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password is required", nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLower(),
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Role = UserRole.Member,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string? firstName, string? lastName, bool? isActive)
    {
        if (firstName != null) FirstName = firstName;
        if (lastName != null) LastName = lastName;
        if (isActive.HasValue) IsActive = isActive.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool VerifyPassword(string password, Func<string, string, bool> verifier)
    {
        return verifier(password, PasswordHash);
    }
}

public enum UserRole
{
    Member,
    Librarian,
    Admin
}
