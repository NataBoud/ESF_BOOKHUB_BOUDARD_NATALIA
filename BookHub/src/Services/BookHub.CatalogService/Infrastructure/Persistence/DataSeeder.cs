using BookHub.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookHub.CatalogService.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(CatalogDbContext context)
    {
        if (await context.Books.AnyAsync()) return;

        var books = new[]
        {
            Book.Create("Clean Code", "Robert C. Martin", "978-0132350884",
                "A Handbook of Agile Software Craftsmanship", "Programming", 2008, 3),
            Book.Create("Design Patterns", "Gang of Four", "978-0201633610",
                "Elements of Reusable Object-Oriented Software", "Programming", 1994, 2),
            Book.Create("Domain-Driven Design", "Eric Evans", "978-0321125217",
                "Tackling Complexity in the Heart of Software", "Architecture", 2003, 2),
            Book.Create("The Pragmatic Programmer", "David Thomas, Andrew Hunt", "978-0135957059",
                "Your Journey to Mastery", "Programming", 2019, 4)
        };

        context.Books.AddRange(books);
        await context.SaveChangesAsync();
    }
}
