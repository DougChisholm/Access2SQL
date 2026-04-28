using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolTests.Web.Infrastructure;

namespace SchoolTests.Web.Pages.Students;

public class IndexModel : PageModel
{
    private readonly SchoolTestsDbContext _db;
    public IndexModel(SchoolTestsDbContext db) => _db = db;

    public List<Domain.Student> Students { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Students = await _db.Students
            .Include(s => s.StudentResults)
            .OrderBy(s => s.StudentName)
            .ToListAsync();
    }
}
