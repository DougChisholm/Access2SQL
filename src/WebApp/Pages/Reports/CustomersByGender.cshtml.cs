using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Pages.Reports;

public class GenderCount
{
    public string Plec { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class CustomersByGenderModel : PageModel
{
    private readonly AppDbContext _db;
    public CustomersByGenderModel(AppDbContext db) => _db = db;

    public IList<GenderCount> Data { get; set; } = new List<GenderCount>();

    public async Task OnGetAsync()
    {
        Data = await _db.Klienci
            .GroupBy(k => k.Plec)
            .Select(g => new GenderCount { Plec = g.Key, Count = g.Count() })
            .ToListAsync();
    }
}
