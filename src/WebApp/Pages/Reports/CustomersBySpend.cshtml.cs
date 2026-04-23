using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Pages.Reports;

public class CustomerSpend
{
    public string Imie { get; set; } = string.Empty;
    public string Nazwisko { get; set; } = string.Empty;
    public decimal TotalSpend { get; set; }
}

public class CustomersBySpendModel : PageModel
{
    private readonly AppDbContext _db;
    public CustomersBySpendModel(AppDbContext db) => _db = db;

    public IList<CustomerSpend> Data { get; set; } = new List<CustomerSpend>();

    public async Task OnGetAsync()
    {
        Data = await _db.Klienci
            .Select(k => new CustomerSpend
            {
                Imie = k.Imie,
                Nazwisko = k.Nazwisko,
                TotalSpend = k.Zamowienia.Sum(z => z.Cena)
            })
            .OrderByDescending(x => x.TotalSpend)
            .Take(50)
            .ToListAsync();
    }
}
