using Microsoft.EntityFrameworkCore;
using WebApp.Infrastructure;
using WebApp.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "schooltests.db");
builder.Services.AddDbContext<SchoolTestsDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

var app = builder.Build();

// Auto-migrate and seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SchoolTestsDbContext>();
    db.Database.EnsureCreated();

    if (!db.Students.Any())
    {
        var students = new List<Student>
        {
            new() { StudentName = "Alice Smith" },
            new() { StudentName = "Bob Jones" },
            new() { StudentName = "Carol White" },
            new() { StudentName = "David Brown" }
        };
        db.Students.AddRange(students);
        db.SaveChanges();

        var tests = new List<Test>
        {
            new() { TestDescription = "Mathematics Term 1" },
            new() { TestDescription = "English Term 1" },
            new() { TestDescription = "Science Term 2" }
        };
        db.Tests.AddRange(tests);
        db.SaveChanges();

        // Seed some results (not all filled in)
        var allStudents = db.Students.ToList();
        var allTests = db.Tests.ToList();

        var results = new List<StudentResult>();
        foreach (var student in allStudents)
        {
            foreach (var test in allTests)
            {
                results.Add(new StudentResult
                {
                    StudentId = student.StudentId,
                    TestId = test.TestId,
                    Mark = null
                });
            }
        }
        // Give some sample marks
        results[0].Mark = 85;
        results[1].Mark = 72;
        results[4].Mark = 90;
        results[5].Mark = 68;

        db.StudentResults.AddRange(results);
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();
