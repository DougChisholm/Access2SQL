using Microsoft.EntityFrameworkCore;
using WebApp.Domain;

namespace WebApp.Infrastructure;

public class BingoDbContext : DbContext
{
    public BingoDbContext(DbContextOptions<BingoDbContext> options) : base(options) { }

    /// <summary>Active game board — cell values are zeroed out as numbers are called.</summary>
    public DbSet<BingoRow> BingoBoard { get; set; } = null!;

    /// <summary>Master backup board — seeded once, never modified during play.</summary>
    public DbSet<BingoRowBak> BingoBoardBak { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BingoRow>(entity =>
        {
            entity.ToTable("tblBingo");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<BingoRowBak>(entity =>
        {
            entity.ToTable("tblBingoBak");
            entity.HasKey(e => e.Id);
        });
    }
}
