using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Customers;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public IList<Klient> Klienci { get; set; } = new List<Klient>();
    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public async Task OnGetAsync()
    {
        var query = _db.Klienci.AsQueryable();
        if (!string.IsNullOrWhiteSpace(Search))
            query = query.Where(k => k.Imie.Contains(Search) || k.Nazwisko.Contains(Search));
        Klienci = await query.OrderBy(k => k.Nazwisko).ThenBy(k => k.Imie).ToListAsync();
    }
}
