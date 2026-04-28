using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolTests.Web.Domain;
using SchoolTests.Web.Infrastructure;

namespace SchoolTests.Web.Pages.Students;

public class CreateModel : PageModel
{
    private readonly SchoolTestsDbContext _db;
    public CreateModel(SchoolTestsDbContext db) => _db = db;

    [BindProperty]
    public Student Student { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        _db.Students.Add(Student);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Student \"{Student.StudentName}\" added successfully.";
        return RedirectToPage("Index");
    }
}
