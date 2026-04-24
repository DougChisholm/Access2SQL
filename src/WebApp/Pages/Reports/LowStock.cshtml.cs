using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Pages.Reports;

public class LowStockItem
{
    public string Nazwa { get; set; } = string.Empty;
    public int Ilosc { get; set; }
}

public class LowStockModel : PageModel
{
    private readonly AppDbContext _db;
    public LowStockModel(AppDbContext db) => _db = db;

    public IList<LowStockItem> Data { get; set; } = new List<LowStockItem>();

    public async Task OnGetAsync()
    {
        Data = await _db.Produkty
            .GroupJoin(_db.StanMagazynu, p => p.IdProduktu, s => s.IdProduktu, (p, stocks) => new { p, stocks })
            .SelectMany(x => x.stocks.DefaultIfEmpty(), (x, s) => new LowStockItem
            {
                Nazwa = x.p.Nazwa,
                Ilosc = s != null ? s.Ilosc : 0
            })
            .Where(item => item.Ilosc < 5)
            .OrderBy(item => item.Ilosc)
            .ToListAsync();
    }
}
