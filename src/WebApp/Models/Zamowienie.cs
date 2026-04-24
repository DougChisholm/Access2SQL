using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

[Table("Zamowienia")]
public class Zamowienie
{
    [Key]
    [Column("id_zamowienia")]
    public int IdZamowienia { get; set; }

    [Column("id_klienta")]
    public int IdKlienta { get; set; }

    [Column("data")]
    public DateOnly Data { get; set; }

    [Column("cena")]
    public decimal Cena { get; set; }

    [Column("kod_promocyjny")]
    public string? KodPromocyjny { get; set; }

    [Column("notatki")]
    public string? Notatki { get; set; }

    public Klient? Klient { get; set; }
    public ICollection<ProduktZamowienia> ProduktyZamowien { get; set; } = new List<ProduktZamowienia>();
}
