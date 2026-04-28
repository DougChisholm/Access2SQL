using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolTests.Web.Infrastructure;

namespace SchoolTests.Web.Pages;

public class ChooseTestModel : PageModel
{
    private readonly SchoolTestsDbContext _db;

    public ChooseTestModel(SchoolTestsDbContext db) => _db = db;

    public List<Domain.Test> Tests { get; private set; } = new();

    /// <summary>
    /// Maps to: SELECT DISTINCTROW TestId, TestDescription FROM Test
    /// (used as the RecordSource for lstTest in the Access ChooseTest form)
    /// </summary>
    public async Task OnGetAsync()
    {
        Tests = await _db.Tests
            .Include(t => t.StudentResults)
            .OrderBy(t => t.TestDescription)
            .ToListAsync();
    }
}
