using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolTests.Web.Domain;
using SchoolTests.Web.Infrastructure;

namespace SchoolTests.Web.Pages.Students;

public class EditModel : PageModel
{
    private readonly SchoolTestsDbContext _db;
    public EditModel(SchoolTestsDbContext db) => _db = db;

    [BindProperty]
    public Student Student { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var s = await _db.Students.FindAsync(id);
        if (s == null) return NotFound();
        Student = s;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var existing = await _db.Students.FindAsync(Student.StudentId);
        if (existing == null) return NotFound();

        existing.StudentName = Student.StudentName;
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Student updated successfully.";
        return RedirectToPage("Index");
    }
}
