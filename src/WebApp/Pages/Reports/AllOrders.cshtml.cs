using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Reports;

public class AllOrdersModel : PageModel
{
    private readonly AppDbContext _db;
    public AllOrdersModel(AppDbContext db) => _db = db;

    public IList<Zamowienie> Orders { get; set; } = new List<Zamowienie>();

    public async Task OnGetAsync()
    {
        Orders = await _db.Zamowienia
            .Include(z => z.Klient)
            .OrderByDescending(z => z.Data)
            .ToListAsync();
    }
}
