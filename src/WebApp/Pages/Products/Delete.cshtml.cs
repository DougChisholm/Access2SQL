using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Products;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;
    public DeleteModel(AppDbContext db) => _db = db;

    public Produkt Produkt { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var produkt = await _db.Produkty.FindAsync(id);
        if (produkt == null) return NotFound();
        Produkt = produkt;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var produkt = await _db.Produkty
            .Include(p => p.ProduktKategorie)
            .Include(p => p.KoszykItems)
            .Include(p => p.ProduktyZamowien)
            .Include(p => p.StanMagazynu)
            .FirstOrDefaultAsync(p => p.IdProduktu == id);
        if (produkt != null)
        {
            _db.Produkty.Remove(produkt);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage("Index");
    }
}
