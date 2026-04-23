using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Products;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public IList<Produkt> Produkty { get; set; } = new List<Produkt>();

    public async Task OnGetAsync()
    {
        Produkty = await _db.Produkty.OrderBy(p => p.Nazwa).ToListAsync();
    }
}
