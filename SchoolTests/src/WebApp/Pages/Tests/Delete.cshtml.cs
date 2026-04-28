using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolTests.Web.Infrastructure;

namespace SchoolTests.Web.Pages.Tests;

public class DeleteModel : PageModel
{
    private readonly SchoolTestsDbContext _db;
    public DeleteModel(SchoolTestsDbContext db) => _db = db;

    public Domain.Test Test { get; private set; } = new();
    public int ResultCount { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var t = await _db.Tests.FindAsync(id);
        if (t == null) return NotFound();
        Test        = t;
        ResultCount = await _db.StudentResults.CountAsync(r => r.TestId == id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var t = await _db.Tests.FindAsync(id);
        if (t != null)
        {
            _db.Tests.Remove(t);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Test \"{t.TestDescription}\" deleted.";
        }
        return RedirectToPage("Index");
    }
}
