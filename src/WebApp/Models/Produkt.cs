using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

[Table("Produkty")]
public class Produkt
{
    [Key]
    [Column("id_produktu")]
    public int IdProduktu { get; set; }

    [Column("nazwa")]
    [Required]
    public string Nazwa { get; set; } = string.Empty;

    [Column("opis")]
    public string? Opis { get; set; }

    [Column("cena")]
    public decimal Cena { get; set; }

    [Column("gwarancja")]
    public int Gwarancja { get; set; }

    [Column("dostepny")]
    public bool Dostepny { get; set; }

    public ICollection<ProduktKategoria> ProduktKategorie { get; set; } = new List<ProduktKategoria>();
    public ICollection<KoszykItem> KoszykItems { get; set; } = new List<KoszykItem>();
    public ICollection<ProduktZamowienia> ProduktyZamowien { get; set; } = new List<ProduktZamowienia>();
    public StanMagazynu? StanMagazynu { get; set; }
}
