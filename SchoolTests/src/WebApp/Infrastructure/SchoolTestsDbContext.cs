using Microsoft.EntityFrameworkCore;
using WebApp.Domain;

namespace WebApp.Infrastructure;

public class SchoolTestsDbContext : DbContext
{
    public SchoolTestsDbContext(DbContextOptions<SchoolTestsDbContext> options)
        : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<StudentResult> StudentResults => Set<StudentResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId);
            entity.Property(e => e.StudentName).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.TestId);
            entity.Property(e => e.TestDescription).IsRequired().HasMaxLength(500);
        });

        modelBuilder.Entity<StudentResult>(entity =>
        {
            entity.HasKey(e => e.ResultId);

            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Results)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Test)
                  .WithMany(t => t.Results)
                  .HasForeignKey(e => e.TestId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
