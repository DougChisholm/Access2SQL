using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Pages.Reports;

public class CityOrders
{
    public string Miasto { get; set; } = string.Empty;
    public int OrderCount { get; set; }
}

public class TopCitiesModel : PageModel
{
    private readonly AppDbContext _db;
    public TopCitiesModel(AppDbContext db) => _db = db;

    public IList<CityOrders> Data { get; set; } = new List<CityOrders>();

    public async Task OnGetAsync()
    {
        Data = await _db.Zamowienia
            .Join(_db.Adresy, z => z.IdKlienta, a => a.IdKlienta, (z, a) => a.Miasto)
            .Where(m => m != null)
            .GroupBy(m => m!)
            .Select(g => new CityOrders { Miasto = g.Key, OrderCount = g.Count() })
            .OrderByDescending(x => x.OrderCount)
            .Take(20)
            .ToListAsync();
    }
}
