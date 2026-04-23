using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

[Table("Produkty_zamowienia")]
public class ProduktZamowienia
{
    [Column("id_zamowienia")]
    public int IdZamowienia { get; set; }

    [Column("id_produktu")]
    public int IdProduktu { get; set; }

    [Column("ilosc")]
    public int Ilosc { get; set; }

    [Column("cena_jednostkowa")]
    public decimal CenaJednostkowa { get; set; }

    public Zamowienie? Zamowienie { get; set; }
    public Produkt? Produkt { get; set; }
}
