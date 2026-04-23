using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

[Table("Koszyk")]
public class KoszykItem
{
    [Column("id_klienta")]
    public int IdKlienta { get; set; }

    [Column("id_produktu")]
    public int IdProduktu { get; set; }

    public Klient? Klient { get; set; }
    public Produkt? Produkt { get; set; }
}
