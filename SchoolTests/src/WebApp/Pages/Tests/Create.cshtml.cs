using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolTests.Web.Infrastructure;

namespace SchoolTests.Web.Pages.Tests;

public class CreateModel : PageModel
{
    private readonly SchoolTestsDbContext _db;
    public CreateModel(SchoolTestsDbContext db) => _db = db;

    [BindProperty]
    public Domain.Test Test { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        _db.Tests.Add(Test);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Test \"{Test.TestDescription}\" created successfully.";
        return RedirectToPage("Index");
    }
}
