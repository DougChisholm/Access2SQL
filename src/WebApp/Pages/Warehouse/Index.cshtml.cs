using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Warehouse;

public class WarehouseItem
{
    public int IdProduktu { get; set; }
    public string Nazwa { get; set; } = string.Empty;
    public int Ilosc { get; set; }
    public bool Dostepny { get; set; }
}

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public IList<WarehouseItem> Items { get; set; } = new List<WarehouseItem>();

    public async Task OnGetAsync()
    {
        Items = await _db.Produkty
            .GroupJoin(_db.StanMagazynu, p => p.IdProduktu, s => s.IdProduktu, (p, stocks) => new { p, stocks })
            .SelectMany(x => x.stocks.DefaultIfEmpty(), (x, s) => new WarehouseItem
            {
                IdProduktu = x.p.IdProduktu,
                Nazwa = x.p.Nazwa,
                Ilosc = s != null ? s.Ilosc : 0,
                Dostepny = x.p.Dostepny
            })
            .OrderBy(i => i.Nazwa)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostSetQuantityAsync(int produktId, int quantity)
    {
        var stan = await _db.StanMagazynu.FindAsync(produktId);
        if (stan == null)
            _db.StanMagazynu.Add(new StanMagazynu { IdProduktu = produktId, Ilosc = quantity });
        else
            stan.Ilosc = quantity;
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAvailabilityAsync()
    {
        var all = await _db.Produkty.ToListAsync();
        var stany = await _db.StanMagazynu.ToDictionaryAsync(s => s.IdProduktu);
        foreach (var p in all)
        {
            int ilosc = stany.TryGetValue(p.IdProduktu, out var s) ? s.Ilosc : 0;
            p.Dostepny = ilosc > 0;
        }
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
