using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolTests.Web.Domain;
using SchoolTests.Web.Infrastructure;

namespace SchoolTests.Web.Pages;

public class IndexModel : PageModel
{
    private readonly SchoolTestsDbContext _db;

    public IndexModel(SchoolTestsDbContext db) => _db = db;

    public int    StudentCount { get; private set; }
    public int    TestCount    { get; private set; }
    public int    ResultCount  { get; private set; }
    public double? AverageMark { get; private set; }

    public List<Domain.Test>    RecentTests    { get; private set; } = new();
    public List<PerformerDto>   TopPerformers  { get; private set; } = new();

    public async Task OnGetAsync()
    {
        StudentCount = await _db.Students.CountAsync();
        TestCount    = await _db.Tests.CountAsync();
        ResultCount  = await _db.StudentResults.CountAsync(r => r.Mark.HasValue);
        AverageMark  = await _db.StudentResults
                            .Where(r => r.Mark.HasValue)
                            .AverageAsync(r => (double?)r.Mark);

        RecentTests = await _db.Tests
            .Include(t => t.StudentResults)
            .OrderBy(t => t.TestId)
            .Take(5)
            .ToListAsync();

        TopPerformers = await _db.Students
            .Select(s => new PerformerDto
            {
                StudentName = s.StudentName,
                TestsTaken  = s.StudentResults.Count(r => r.Mark.HasValue),
                AvgMark     = s.StudentResults.Any(r => r.Mark.HasValue)
                    ? s.StudentResults.Where(r => r.Mark.HasValue).Average(r => (double)r.Mark!)
                    : 0
            })
            .Where(p => p.TestsTaken > 0)
            .OrderByDescending(p => p.AvgMark)
            .Take(5)
            .ToListAsync();
    }

    public record PerformerDto
    {
        public string StudentName { get; init; } = "";
        public int    TestsTaken  { get; init; }
        public double AvgMark     { get; init; }
    }
}
