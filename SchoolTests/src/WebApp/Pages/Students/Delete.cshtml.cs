using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolTests.Web.Domain;
using SchoolTests.Web.Infrastructure;

namespace SchoolTests.Web.Pages.Students;

public class DeleteModel : PageModel
{
    private readonly SchoolTestsDbContext _db;
    public DeleteModel(SchoolTestsDbContext db) => _db = db;

    public Student Student { get; private set; } = new();
    public int ResultCount { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var s = await _db.Students.FindAsync(id);
        if (s == null) return NotFound();
        Student     = s;
        ResultCount = await _db.StudentResults.CountAsync(r => r.StudentId == id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var s = await _db.Students.FindAsync(id);
        if (s != null)
        {
            _db.Students.Remove(s);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Student \"{s.StudentName}\" deleted.";
        }
        return RedirectToPage("Index");
    }
}
