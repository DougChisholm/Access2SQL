using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

[Table("Klienci")]
public class Klient
{
    [Key]
    [Column("id_klienta")]
    public int IdKlienta { get; set; }

    [Column("imie")]
    [Required]
    public string Imie { get; set; } = string.Empty;

    [Column("nazwisko")]
    [Required]
    public string Nazwisko { get; set; } = string.Empty;

    [Column("data_urodzenia")]
    public DateOnly DataUrodzenia { get; set; }

    [Column("plec")]
    [Required]
    public string Plec { get; set; } = string.Empty;

    [Column("pesel")]
    public string? Pesel { get; set; }

    public ICollection<Adres> Adresy { get; set; } = new List<Adres>();
    public ICollection<KoszykItem> Koszyk { get; set; } = new List<KoszykItem>();
    public ICollection<Zamowienie> Zamowienia { get; set; } = new List<Zamowienie>();
}
