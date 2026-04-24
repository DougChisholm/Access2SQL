using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Pages.Reports;

public class AgeGroupSpend
{
    public string AgeGroup { get; set; } = string.Empty;
    public decimal TotalSpend { get; set; }
}

public class ExpensesByAgeModel : PageModel
{
    private readonly AppDbContext _db;
    public ExpensesByAgeModel(AppDbContext db) => _db = db;

    public IList<AgeGroupSpend> Data { get; set; } = new List<AgeGroupSpend>();

    public async Task OnGetAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var klienci = await _db.Klienci
            .Select(k => new { k.DataUrodzenia, TotalSpend = k.Zamowienia.Sum(z => z.Cena) })
            .ToListAsync();

        Data = klienci
            .Select(k => new
            {
                Age = today.Year - k.DataUrodzenia.Year -
                      (today.DayOfYear < k.DataUrodzenia.DayOfYear ? 1 : 0),
                k.TotalSpend
            })
            .GroupBy(x => x.Age switch
            {
                < 18 => "< 18",
                < 31 => "18-30",
                < 41 => "31-40",
                < 51 => "41-50",
                < 61 => "51-60",
                _ => "61+"
            })
            .Select(g => new AgeGroupSpend { AgeGroup = g.Key, TotalSpend = g.Sum(x => x.TotalSpend) })
            .OrderBy(x => x.AgeGroup)
            .ToList();
    }
}
