using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolTests.Web.Domain;

namespace SchoolTests.Web.Infrastructure;

public class SchoolTestsDbContext : IdentityDbContext<IdentityUser>
{
    public SchoolTestsDbContext(DbContextOptions<SchoolTestsDbContext> options)
        : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Domain.Test> Tests => Set<Domain.Test>();
    public DbSet<StudentResult> StudentResults => Set<StudentResult>();
    public DbSet<AppInfo> AppInfos => Set<AppInfo>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Student>(e =>
        {
            e.ToTable("Student");
            e.HasKey(s => s.StudentId);
            e.Property(s => s.StudentName).IsRequired().HasMaxLength(100);
        });

        builder.Entity<Domain.Test>(e =>
        {
            e.ToTable("Test");
            e.HasKey(t => t.TestId);
            e.Property(t => t.TestDescription).IsRequired().HasMaxLength(200);
        });

        builder.Entity<StudentResult>(e =>
        {
            e.ToTable("StudentResult");
            e.HasKey(r => r.ResultId);
            e.HasOne(r => r.Student)
             .WithMany(s => s.StudentResults)
             .HasForeignKey(r => r.StudentId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(r => r.Test)
             .WithMany(t => t.StudentResults)
             .HasForeignKey(r => r.TestId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AppInfo>(e =>
        {
            e.ToTable("README");
            e.HasKey(a => a.Item);
        });
    }
}
