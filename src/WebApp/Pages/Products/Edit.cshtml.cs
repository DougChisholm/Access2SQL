using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Products;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    public EditModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Produkt Produkt { get; set; } = new();

    public IList<ProduktKategoria> ProduktKategorie { get; set; } = new List<ProduktKategoria>();
    public IList<Kategoria> AllKategorie { get; set; } = new List<Kategoria>();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var produkt = await _db.Produkty.FindAsync(id);
        if (produkt == null) return NotFound();
        Produkt = produkt;
        await LoadKategorieAsync(id);
        return Page();
    }

    private async Task LoadKategorieAsync(int id)
    {
        ProduktKategorie = await _db.ProduktyKategorie
            .Include(pk => pk.Kategoria)
            .Where(pk => pk.IdProduktu == id)
            .ToListAsync();
        AllKategorie = await _db.Kategorie.OrderBy(k => k.Nazwa).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadKategorieAsync(Produkt.IdProduktu);
            return Page();
        }
        _db.Produkty.Update(Produkt);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostAddCategoryAsync(int id, int kategoriaId)
    {
        var exists = await _db.ProduktyKategorie.FindAsync(id, kategoriaId);
        if (exists == null)
        {
            _db.ProduktyKategorie.Add(new ProduktKategoria { IdProduktu = id, IdKategorii = kategoriaId });
            await _db.SaveChangesAsync();
        }
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRemoveCategoryAsync(int id, int kategoriaId)
    {
        var pk = await _db.ProduktyKategorie.FindAsync(id, kategoriaId);
        if (pk != null)
        {
            _db.ProduktyKategorie.Remove(pk);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage(new { id });
    }
}
