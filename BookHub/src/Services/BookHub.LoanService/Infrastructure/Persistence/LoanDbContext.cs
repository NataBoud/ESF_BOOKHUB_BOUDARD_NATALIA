using BookHub.LoanService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookHub.LoanService.Infrastructure.Persistence;

public class LoanDbContext : DbContext
{
    public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options)
    {
    }

    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BookTitle).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UserEmail).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PenaltyAmount).HasPrecision(10, 2);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.BookId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.DueDate);
        });
    }
}
