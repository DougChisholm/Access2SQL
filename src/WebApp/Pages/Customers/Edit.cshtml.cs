using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Customers;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    public EditModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Klient Klient { get; set; } = new();

    [BindProperty]
    public Adres NewAdres { get; set; } = new();

    public IList<Adres> Adresy { get; set; } = new List<Adres>();
    public IList<KoszykItem> KoszykItems { get; set; } = new List<KoszykItem>();
    public IList<Produkt> AllProducts { get; set; } = new List<Produkt>();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var klient = await _db.Klienci.FindAsync(id);
        if (klient == null) return NotFound();
        Klient = klient;
        await LoadRelatedDataAsync(id);
        return Page();
    }

    private async Task LoadRelatedDataAsync(int id)
    {
        Adresy = await _db.Adresy.Where(a => a.IdKlienta == id).ToListAsync();
        KoszykItems = await _db.Koszyk.Include(ki => ki.Produkt).Where(ki => ki.IdKlienta == id).ToListAsync();
        AllProducts = await _db.Produkty.OrderBy(p => p.Nazwa).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadRelatedDataAsync(Klient.IdKlienta);
            return Page();
        }
        _db.Klienci.Update(Klient);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostAddAddressAsync()
    {
        _db.Adresy.Add(NewAdres);
        await _db.SaveChangesAsync();
        return RedirectToPage(new { id = NewAdres.IdKlienta });
    }

    public async Task<IActionResult> OnPostDeleteAddressAsync(int adresId)
    {
        var adres = await _db.Adresy.FindAsync(adresId);
        if (adres != null)
        {
            int klientId = adres.IdKlienta;
            _db.Adresy.Remove(adres);
            await _db.SaveChangesAsync();
            return RedirectToPage(new { id = klientId });
        }
        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostAddToCartAsync(int id, int produktId)
    {
        var existing = await _db.Koszyk.FindAsync(id, produktId);
        if (existing == null)
        {
            _db.Koszyk.Add(new KoszykItem { IdKlienta = id, IdProduktu = produktId });
            await _db.SaveChangesAsync();
        }
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRemoveFromCartAsync(int id, int produktId)
    {
        var item = await _db.Koszyk.FindAsync(id, produktId);
        if (item != null)
        {
            _db.Koszyk.Remove(item);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostClearCartAsync(int id)
    {
        var items = await _db.Koszyk.Where(ki => ki.IdKlienta == id).ToListAsync();
        _db.Koszyk.RemoveRange(items);
        await _db.SaveChangesAsync();
        return RedirectToPage(new { id });
    }
}
