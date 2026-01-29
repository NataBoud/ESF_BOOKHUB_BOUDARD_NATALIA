using BookHub.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookHub.CatalogService.Infrastructure.Persistence;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Author).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ISBN).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.ISBN).IsUnique();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Category).HasMaxLength(50);
        });
    }
}
