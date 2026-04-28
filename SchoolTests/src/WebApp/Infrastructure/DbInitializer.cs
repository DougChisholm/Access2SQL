using Microsoft.AspNetCore.Identity;
using SchoolTests.Web.Domain;

namespace SchoolTests.Web.Infrastructure;

public static class DbInitializer
{
    public static async System.Threading.Tasks.Task SeedAsync(
        SchoolTestsDbContext context,
        UserManager<IdentityUser> userManager)
    {
        // Seed AppInfo metadata
        if (!context.AppInfos.Any())
        {
            context.AppInfos.AddRange(
                new AppInfo { Item = "AppName",    Comment = "School Test Results System" },
                new AppInfo { Item = "Version",    Comment = "1.0" },
                new AppInfo { Item = "MigratedFrom", Comment = "Microsoft Access" }
            );
            await context.SaveChangesAsync();
        }

        // Seed students
        if (!context.Students.Any())
        {
            context.Students.AddRange(
                new Student { StudentName = "Alice Johnson" },
                new Student { StudentName = "Bob Smith" },
                new Student { StudentName = "Charlie Brown" },
                new Student { StudentName = "Diana Prince" },
                new Student { StudentName = "Eve Wilson" },
                new Student { StudentName = "Frank Castle" }
            );
            await context.SaveChangesAsync();
        }

        // Seed tests
        if (!context.Tests.Any())
        {
            context.Tests.AddRange(
                new Domain.Test { TestDescription = "Mathematics — Chapter 1: Algebra" },
                new Domain.Test { TestDescription = "English — Essay Writing" },
                new Domain.Test { TestDescription = "Science — Lab Report" },
                new Domain.Test { TestDescription = "History — World War II" }
            );
            await context.SaveChangesAsync();
        }

        // Seed some sample results
        if (!context.StudentResults.Any())
        {
            var students = context.Students.ToList();
            var tests    = context.Tests.ToList();

            if (students.Count >= 4 && tests.Count >= 2)
            {
                var mathTest    = tests[0];
                var englishTest = tests[1];

                var sampleResults = new[]
                {
                    new StudentResult { StudentId = students[0].StudentId, TestId = mathTest.TestId,    Mark = 87 },
                    new StudentResult { StudentId = students[1].StudentId, TestId = mathTest.TestId,    Mark = 74 },
                    new StudentResult { StudentId = students[2].StudentId, TestId = mathTest.TestId,    Mark = 91 },
                    new StudentResult { StudentId = students[3].StudentId, TestId = mathTest.TestId,    Mark = 68 },
                    new StudentResult { StudentId = students[0].StudentId, TestId = englishTest.TestId, Mark = 82 },
                    new StudentResult { StudentId = students[1].StudentId, TestId = englishTest.TestId, Mark = 79 },
                    new StudentResult { StudentId = students[2].StudentId, TestId = englishTest.TestId, Mark = 95 },
                };
                context.StudentResults.AddRange(sampleResults);
                await context.SaveChangesAsync();
            }
        }

        // Seed default admin user
        const string adminEmail    = "admin@schooltests.local";
        const string adminPassword = "Admin123!";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new IdentityUser
            {
                UserName = adminEmail,
                Email    = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, adminPassword);
        }
    }
}
