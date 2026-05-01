using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain;
using WebApp.Infrastructure;

namespace WebApp.Pages.Tests;

public class ManageModel : PageModel
{
    private readonly SchoolTestsDbContext _db;
    public ManageModel(SchoolTestsDbContext db) => _db = db;

    public List<Test> Tests { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? EditId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Description is required.")]
    [StringLength(500, MinimumLength = 2, ErrorMessage = "Description must be between 2 and 500 characters.")]
    public string NewDescription { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        Tests = await _db.Tests.OrderBy(t => t.TestDescription).ToListAsync();

        if (EditId.HasValue)
        {
            var test = await _db.Tests.FindAsync(EditId.Value);
            if (test != null) NewDescription = test.TestDescription;
        }
    }

    public async Task<IActionResult> OnPostSaveAsync([FromForm] int? EditId)
    {
        if (!ModelState.IsValid)
        {
            Tests = await _db.Tests.OrderBy(t => t.TestDescription).ToListAsync();
            this.EditId = EditId;
            return Page();
        }

        if (EditId.HasValue)
        {
            var test = await _db.Tests.FindAsync(EditId.Value);
            if (test != null)
            {
                test.TestDescription = NewDescription.Trim();
                await _db.SaveChangesAsync();
                TempData["Success"] = "Test updated successfully.";
            }
        }
        else
        {
            _db.Tests.Add(new Test { TestDescription = NewDescription.Trim() });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Test added successfully.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int deleteId)
    {
        var test = await _db.Tests.FindAsync(deleteId);
        if (test != null)
        {
            _db.Tests.Remove(test);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Test deleted.";
        }
        return RedirectToPage();
    }
}
