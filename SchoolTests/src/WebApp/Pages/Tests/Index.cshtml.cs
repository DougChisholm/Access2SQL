using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Infrastructure;

namespace WebApp.Pages.Tests;

public class TestSummary
{
    public int TestId { get; set; }
    public string TestDescription { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int MarkedCount { get; set; }
}

public class IndexModel : PageModel
{
    private readonly SchoolTestsDbContext _db;
    public IndexModel(SchoolTestsDbContext db) => _db = db;

    public List<TestSummary> Tests { get; set; } = new();

    public async Task OnGetAsync()
    {
        var totalStudents = await _db.Students.CountAsync();

        Tests = await _db.Tests
            .Select(t => new TestSummary
            {
                TestId = t.TestId,
                TestDescription = t.TestDescription,
                TotalStudents = totalStudents,
                MarkedCount = t.Results.Count(r => r.Mark != null)
            })
            .OrderBy(t => t.TestDescription)
            .ToListAsync();
    }
}
