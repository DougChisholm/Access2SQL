using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Pages.Reports;

public class ProductSales
{
    public string Nazwa { get; set; } = string.Empty;
    public int TotalQty { get; set; }
}

public class TopProductsModel : PageModel
{
    private readonly AppDbContext _db;
    public TopProductsModel(AppDbContext db) => _db = db;

    public IList<ProductSales> Data { get; set; } = new List<ProductSales>();

    public async Task OnGetAsync()
    {
        Data = await _db.ProduktyZamowien
            .GroupBy(pz => pz.IdProduktu)
            .Select(g => new { IdProduktu = g.Key, TotalQty = g.Sum(x => x.Ilosc) })
            .Join(_db.Produkty, x => x.IdProduktu, p => p.IdProduktu, (x, p) => new ProductSales
            {
                Nazwa = p.Nazwa,
                TotalQty = x.TotalQty
            })
            .OrderByDescending(x => x.TotalQty)
            .Take(20)
            .ToListAsync();
    }
}
