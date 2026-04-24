using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

[Table("Stan_Magazynu")]
public class StanMagazynu
{
    [Column("id_produktu")]
    public int IdProduktu { get; set; }

    [Column("ilosc")]
    public int Ilosc { get; set; }

    public Produkt? Produkt { get; set; }
}
