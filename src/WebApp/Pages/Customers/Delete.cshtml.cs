using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Customers;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;
    public DeleteModel(AppDbContext db) => _db = db;

    public Klient Klient { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var klient = await _db.Klienci.FindAsync(id);
        if (klient == null) return NotFound();
        Klient = klient;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var klient = await _db.Klienci
            .Include(k => k.Adresy)
            .Include(k => k.Koszyk)
            .Include(k => k.Zamowienia).ThenInclude(z => z.ProduktyZamowien)
            .FirstOrDefaultAsync(k => k.IdKlienta == id);
        if (klient != null)
        {
            _db.Klienci.Remove(klient);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage("Index");
    }
}
