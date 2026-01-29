using BookHub.UserService.Domain.Entities;
using BookHub.UserService.Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace BookHub.UserService.Infrastructure.Persistence;

public class DataSeeder
{
    private readonly UserDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public DataSeeder(UserDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        if (await _context.Users.AnyAsync()) return;

        var users = new[]
        {
            User.Create("admin@bookhub.com", _passwordHasher.Hash("Admin123!"), "Admin", "BookHub"),
            User.Create("librarian@bookhub.com", _passwordHasher.Hash("Librarian123!"), "Marie", "Dupont"),
            User.Create("user@bookhub.com", _passwordHasher.Hash("User123!"), "Jean", "Martin")
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();
    }
}
