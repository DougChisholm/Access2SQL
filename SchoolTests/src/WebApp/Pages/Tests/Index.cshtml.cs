using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolTests.Web.Infrastructure;

namespace SchoolTests.Web.Pages.Tests;

public class IndexModel : PageModel
{
    private readonly SchoolTestsDbContext _db;
    public IndexModel(SchoolTestsDbContext db) => _db = db;

    public List<Domain.Test> Tests { get; private set; } = new();
    public int StudentCount { get; private set; }

    public async Task OnGetAsync()
    {
        Tests = await _db.Tests
            .Include(t => t.StudentResults)
            .OrderBy(t => t.TestDescription)
            .ToListAsync();

        StudentCount = await _db.Students.CountAsync();
    }
}
