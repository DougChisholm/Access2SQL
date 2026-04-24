using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.PriceReduction;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public SelectList ProduktSelectList { get; set; } = new SelectList(Enumerable.Empty<object>());
    public Produkt? SelectedProdukt { get; set; }
    public decimal? NewPrice { get; set; }
    public decimal Discount { get; set; }

    private async Task LoadProductsAsync()
    {
        var products = await _db.Produkty.OrderBy(p => p.Nazwa).ToListAsync();
        ProduktSelectList = new SelectList(products, "IdProduktu", "Nazwa");
    }

    public async Task OnGetAsync()
    {
        await LoadProductsAsync();
    }

    public async Task<IActionResult> OnPostPreviewAsync(int produktId, decimal discount)
    {
        await LoadProductsAsync();
        Discount = discount;
        SelectedProdukt = await _db.Produkty.FindAsync(produktId);
        if (SelectedProdukt != null)
            NewPrice = SelectedProdukt.Cena * (1 - discount);
        return Page();
    }

    public async Task<IActionResult> OnPostApplyAsync(int produktId, decimal discount)
    {
        var produkt = await _db.Produkty.FindAsync(produktId);
        if (produkt != null)
        {
            produkt.Cena = produkt.Cena * (1 - discount);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Cena produktu '{produkt.Nazwa}' zaktualizowana do {produkt.Cena:C}";
        }
        await LoadProductsAsync();
        Discount = discount;
        return Page();
    }
}
