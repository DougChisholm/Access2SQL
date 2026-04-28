using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolTests.Web.Infrastructure;

namespace SchoolTests.Web.Pages.Tests;

public class EditModel : PageModel
{
    private readonly SchoolTestsDbContext _db;
    public EditModel(SchoolTestsDbContext db) => _db = db;

    [BindProperty]
    public Domain.Test Test { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var t = await _db.Tests.FindAsync(id);
        if (t == null) return NotFound();
        Test = t;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var existing = await _db.Tests.FindAsync(Test.TestId);
        if (existing == null) return NotFound();

        existing.TestDescription = Test.TestDescription;
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Test updated successfully.";
        return RedirectToPage("Index");
    }
}
