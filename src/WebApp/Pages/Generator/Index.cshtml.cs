using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Generator;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    [BindProperty]
    [Range(1, 10000)]
    public int Count { get; set; } = 10;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var rand = new Random();

        var imionaDamskie = await _db.ImionaDamskie.Select(i => i.Imie).ToListAsync();
        var imionaMeskie = await _db.ImionaMeskie.Select(i => i.Imie).ToListAsync();
        var nazwiskaDamskie = await _db.NazwiskaDamskie.Select(n => n.Nazwisko).ToListAsync();
        var nazwiskaMeskie = await _db.NazwiskaMeskie.Select(n => n.Nazwisko).ToListAsync();
        var adresy = await _db.PrzykladoweAdresy.ToListAsync();
        var produkty = await _db.Produkty.Select(p => new { p.IdProduktu, p.Cena }).ToListAsync();

        if (imionaDamskie.Count == 0 && imionaMeskie.Count == 0)
        {
            TempData["Error"] = "Brak danych w tabelach imion. Najpierw zasilij bazę danych.";
            return Page();
        }

        for (int i = 0; i < Count; i++)
        {
            bool isMale = rand.Next(2) == 0;
            string plec = isMale ? "M" : "K";

            string imie;
            string nazwisko;

            if (isMale && imionaMeskie.Count > 0)
                imie = imionaMeskie[rand.Next(imionaMeskie.Count)];
            else if (!isMale && imionaDamskie.Count > 0)
                imie = imionaDamskie[rand.Next(imionaDamskie.Count)];
            else
                imie = isMale ? "Jan" : "Anna";

            if (isMale && nazwiskaMeskie.Count > 0)
                nazwisko = nazwiskaMeskie[rand.Next(nazwiskaMeskie.Count)];
            else if (!isMale && nazwiskaDamskie.Count > 0)
                nazwisko = nazwiskaDamskie[rand.Next(nazwiskaDamskie.Count)];
            else
                nazwisko = isMale ? "Kowalski" : "Kowalska";

            int birthYear = rand.Next(1950, 2003);
            int birthMonth = rand.Next(1, 13);
            int maxDay = DateTime.DaysInMonth(birthYear, birthMonth);
            int birthDay = rand.Next(1, maxDay + 1);
            var dataUrodzenia = new DateOnly(birthYear, birthMonth, birthDay);

            string pesel = GeneratePesel(dataUrodzenia, isMale, rand);

            var klient = new Klient
            {
                Imie = imie,
                Nazwisko = nazwisko,
                DataUrodzenia = dataUrodzenia,
                Plec = plec,
                Pesel = pesel
            };
            _db.Klienci.Add(klient);
            await _db.SaveChangesAsync();

            if (adresy.Count > 0)
            {
                var adres = adresy[rand.Next(adresy.Count)];
                _db.Adresy.Add(new Adres
                {
                    IdKlienta = klient.IdKlienta,
                    Kraj = adres.Kraj,
                    Miasto = adres.Miasto,
                    KodPocztowy = adres.KodPocztowy,
                    Ulica = adres.Ulica,
                    NrDomu = adres.NrDomu
                });
            }

            int age = DateTime.Now.Year - birthYear;
            int orderCount = rand.Next(1, 7);
            for (int o = 0; o < orderCount; o++)
            {
                int productCount = age switch
                {
                    < 30 => rand.Next(1, 4),
                    < 40 => rand.Next(1, 6),
                    _ => rand.Next(1, 3)
                };

                decimal totalCena = 0;
                var orderDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-rand.Next(0, 365)));
                var zamowienie = new Zamowienie
                {
                    IdKlienta = klient.IdKlienta,
                    Data = orderDate,
                    Cena = 0
                };
                _db.Zamowienia.Add(zamowienie);
                await _db.SaveChangesAsync();

                if (produkty.Count > 0)
                {
                    var selectedProducts = new HashSet<int>();
                    for (int p = 0; p < productCount; p++)
                    {
                        var prod = produkty[rand.Next(produkty.Count)];
                        if (selectedProducts.Add(prod.IdProduktu))
                        {
                            int ilosc = rand.Next(1, 4);
                            _db.ProduktyZamowien.Add(new ProduktZamowienia
                            {
                                IdZamowienia = zamowienie.IdZamowienia,
                                IdProduktu = prod.IdProduktu,
                                Ilosc = ilosc,
                                CenaJednostkowa = prod.Cena
                            });
                            totalCena += prod.Cena * ilosc;
                        }
                    }
                    zamowienie.Cena = totalCena;
                }
            }

            await _db.SaveChangesAsync();
        }

        TempData["Result"] = $"Wygenerowano {Count} klientów z zamówieniami.";
        return RedirectToPage();
    }

    private static string GeneratePesel(DateOnly date, bool isMale, Random rand)
    {
        int yy = date.Year % 100;
        int mm = date.Month;
        int dd = date.Day;

        if (date.Year >= 2000 && date.Year < 2100) mm += 20;
        else if (date.Year >= 1900 && date.Year < 2000) { /* no change */ }

        string digits = $"{yy:D2}{mm:D2}{dd:D2}";
        digits += $"{rand.Next(0, 10)}{rand.Next(0, 10)}{rand.Next(0, 10)}";
        int genderDigit = isMale ? rand.Next(0, 5) * 2 + 1 : rand.Next(0, 5) * 2;
        digits += genderDigit.ToString();

        int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (digits[i] - '0') * weights[i];
        int control = (10 - sum % 10) % 10;
        return digits + control.ToString();
    }
}
