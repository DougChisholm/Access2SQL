using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolTests.Web.Domain;
using SchoolTests.Web.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace SchoolTests.Web.Pages;

/// <summary>
/// Equivalent to the Access "StudentResult" form.
///
/// The original form was based on the "StudentInTest" query which did:
///   SELECT FilterResult.ResultId, FilterResult.StudentId, Student.StudentName,
///          FilterResult.TestId, FilterResult.Mark
///   FROM Student LEFT JOIN FilterResult ON Student.StudentId = FilterResult.StudentId
///   ORDER BY Student.StudentName
///
/// Where FilterResult = SELECT * FROM StudentResult WHERE TestId = [forms]![ChooseTest]![lstTest]
///
/// Translated logic:
///   - GET:  left-join all Students with their result for this TestId
///   - POST: upsert (insert if new, update if existing) each mark entry
///           (mirrors Mark_BeforeUpdate which set TestId from ChooseTest form)
/// </summary>
public class EnterResultsModel : PageModel
{
    private readonly SchoolTestsDbContext _db;

    public EnterResultsModel(SchoolTestsDbContext db) => _db = db;

    [BindProperty]
    public int TestId { get; set; }

    [BindProperty]
    public List<StudentMarkEntry> Entries { get; set; } = new();

    public string TestDescription { get; private set; } = string.Empty;

    // ── GET: load all students, left-join with existing results ──────────────
    public async Task<IActionResult> OnGetAsync(int testId)
    {
        var test = await _db.Tests.FindAsync(testId);
        if (test == null) return RedirectToPage("/ChooseTest");

        TestId          = testId;
        TestDescription = test.TestDescription;

        // Mirrors the StudentInTest query: LEFT JOIN Student ← FilterResult
        Entries = await (
            from s in _db.Students
            join r in _db.StudentResults.Where(x => x.TestId == testId)
                on s.StudentId equals r.StudentId into results
            from r in results.DefaultIfEmpty()
            orderby s.StudentName
            select new StudentMarkEntry
            {
                ResultId    = r != null ? (int?)r.ResultId : null,
                StudentId   = s.StudentId,
                StudentName = s.StudentName,
                Mark        = r != null ? r.Mark : null
            }
        ).ToListAsync();

        return Page();
    }

    // ── POST: save / upsert marks ─────────────────────────────────────────────
    public async Task<IActionResult> OnPostAsync()
    {
        // Re-load description for display
        var test = await _db.Tests.FindAsync(TestId);
        if (test == null) return RedirectToPage("/ChooseTest");
        TestDescription = test.TestDescription;

        if (!ModelState.IsValid) return Page();

        int saved = 0;

        foreach (var entry in Entries)
        {
            if (entry.ResultId.HasValue)
            {
                // Update existing row
                var existing = await _db.StudentResults.FindAsync(entry.ResultId.Value);
                if (existing != null)
                {
                    existing.Mark = entry.Mark;
                    saved++;
                }
            }
            else if (entry.Mark.HasValue)
            {
                // Insert new row — mirrors Mark_BeforeUpdate which sets TestId from ChooseTest
                _db.StudentResults.Add(new StudentResult
                {
                    StudentId = entry.StudentId,
                    TestId    = TestId,
                    Mark      = entry.Mark.Value
                });
                saved++;
            }
        }

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Results saved for {saved} student(s).";
        return RedirectToPage(new { testId = TestId });
    }
}

/// <summary>
/// View-model row: one student's mark entry for a given test.
/// Equivalent to a record in the StudentInTest query result set.
/// </summary>
public class StudentMarkEntry
{
    public int?   ResultId    { get; set; }
    public int    StudentId   { get; set; }
    public string StudentName { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "Mark must be between 0 and 100.")]
    public int?   Mark        { get; set; }
}
