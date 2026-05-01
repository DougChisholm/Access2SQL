using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Infrastructure;

namespace WebApp.Pages.Results;

public class ResultRow
{
    public int ResultId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int? Mark { get; set; }
}

public class EnterModel : PageModel
{
    private readonly SchoolTestsDbContext _db;
    public EnterModel(SchoolTestsDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int TestId { get; set; }

    public string TestDescription { get; set; } = string.Empty;
    public List<ResultRow> Rows { get; set; } = new();
    public int MarkedCount => Rows.Count(r => r.Mark.HasValue);

    public async Task<IActionResult> OnGetAsync()
    {
        var test = await _db.Tests.FindAsync(TestId);
        if (test == null) return RedirectToPage("/Tests/Index");

        TestDescription = test.TestDescription;

        // Ensure every student has a result row for this test (LEFT JOIN equivalent)
        var students = await _db.Students.OrderBy(s => s.StudentName).ToListAsync();
        var existing = await _db.StudentResults
            .Where(r => r.TestId == TestId)
            .ToDictionaryAsync(r => r.StudentId);

        // Create missing result rows
        var toAdd = new List<Domain.StudentResult>();
        foreach (var student in students)
        {
            if (!existing.ContainsKey(student.StudentId))
            {
                var newResult = new Domain.StudentResult
                {
                    StudentId = student.StudentId,
                    TestId = TestId,
                    Mark = null
                };
                toAdd.Add(newResult);
            }
        }

        if (toAdd.Count > 0)
        {
            _db.StudentResults.AddRange(toAdd);
            await _db.SaveChangesAsync();
            // Reload
            existing = await _db.StudentResults
                .Where(r => r.TestId == TestId)
                .ToDictionaryAsync(r => r.StudentId);
        }

        Rows = students.Select(s => new ResultRow
        {
            ResultId = existing.TryGetValue(s.StudentId, out var r) ? r.ResultId : 0,
            StudentId = s.StudentId,
            StudentName = s.StudentName,
            Mark = existing.TryGetValue(s.StudentId, out var r2) ? r2.Mark : null
        }).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int TestId, List<ResultRow> Rows)
    {
        var test = await _db.Tests.FindAsync(TestId);
        if (test == null) return RedirectToPage("/Tests/Index");

        TestDescription = test.TestDescription;

        foreach (var row in Rows)
        {
            if (row.Mark.HasValue && (row.Mark < 0 || row.Mark > 100))
            {
                ModelState.AddModelError(string.Empty, $"Mark for {row.StudentName} must be between 0 and 100.");
            }
        }

        if (!ModelState.IsValid)
        {
            this.Rows = Rows;
            return Page();
        }

        // Bulk update
        var resultIds = Rows.Select(r => r.ResultId).ToList();
        var dbResults = await _db.StudentResults
            .Where(r => r.TestId == TestId)
            .ToDictionaryAsync(r => r.ResultId);

        foreach (var row in Rows)
        {
            if (dbResults.TryGetValue(row.ResultId, out var dbRow))
            {
                dbRow.Mark = row.Mark;
            }
        }

        await _db.SaveChangesAsync();

        TempData["Success"] = "Marks saved successfully.";
        return RedirectToPage(new { testId = TestId });
    }
}
