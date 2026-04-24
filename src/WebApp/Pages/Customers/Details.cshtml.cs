using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Customers;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _db;
    public DetailsModel(AppDbContext db) => _db = db;

    public Klient Klient { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var klient = await _db.Klienci
            .Include(k => k.Adresy)
            .Include(k => k.Koszyk).ThenInclude(ki => ki.Produkt)
            .Include(k => k.Zamowienia)
            .FirstOrDefaultAsync(k => k.IdKlienta == id);
        if (klient == null) return NotFound();
        Klient = klient;
        return Page();
    }
}
