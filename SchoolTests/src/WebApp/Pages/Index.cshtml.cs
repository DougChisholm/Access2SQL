using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Infrastructure;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly SchoolTestsDbContext _db;
    public IndexModel(SchoolTestsDbContext db) => _db = db;

    public int StudentCount { get; set; }
    public int TestCount { get; set; }
    public int MarkedCount { get; set; }

    public async Task OnGetAsync()
    {
        StudentCount = await _db.Students.CountAsync();
        TestCount = await _db.Tests.CountAsync();
        MarkedCount = await _db.StudentResults.CountAsync(r => r.Mark != null);
    }
}
