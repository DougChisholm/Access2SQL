using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain;
using WebApp.Infrastructure;

namespace WebApp.Pages.Students;

public class IndexModel : PageModel
{
    private readonly SchoolTestsDbContext _db;
    public IndexModel(SchoolTestsDbContext db) => _db = db;

    public List<Student> Students { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? EditId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters.")]
    public string NewName { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        Students = await _db.Students.OrderBy(s => s.StudentName).ToListAsync();
        if (EditId.HasValue)
        {
            var s = await _db.Students.FindAsync(EditId.Value);
            if (s != null) NewName = s.StudentName;
        }
    }

    public async Task<IActionResult> OnPostSaveAsync([FromForm] int? EditId)
    {
        if (!ModelState.IsValid)
        {
            Students = await _db.Students.OrderBy(s => s.StudentName).ToListAsync();
            this.EditId = EditId;
            return Page();
        }

        if (EditId.HasValue)
        {
            var s = await _db.Students.FindAsync(EditId.Value);
            if (s != null)
            {
                s.StudentName = NewName.Trim();
                await _db.SaveChangesAsync();
                TempData["Success"] = "Student updated.";
            }
        }
        else
        {
            _db.Students.Add(new Student { StudentName = NewName.Trim() });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Student added.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int deleteId)
    {
        var s = await _db.Students.FindAsync(deleteId);
        if (s != null)
        {
            _db.Students.Remove(s);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Student deleted.";
        }
        return RedirectToPage();
    }
}
