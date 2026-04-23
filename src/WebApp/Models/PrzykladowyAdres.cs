using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

[Table("przykladowe_adresy")]
public class PrzykladowyAdres
{
    [Key]
    [Column("id_adresu")]
    public int IdAdresu { get; set; }

    [Column("id_klienta")]
    public int? IdKlienta { get; set; }

    [Column("kraj")]
    public string? Kraj { get; set; }

    [Column("miasto")]
    public string? Miasto { get; set; }

    [Column("kod_pocztowy")]
    public string? KodPocztowy { get; set; }

    [Column("ulica")]
    public string? Ulica { get; set; }

    [Column("nr_domu")]
    public string? NrDomu { get; set; }
}
