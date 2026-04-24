using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

[Table("Produkty_Kategorie")]
public class ProduktKategoria
{
    [Column("id_produktu")]
    public int IdProduktu { get; set; }

    [Column("id_kategorii")]
    public int IdKategorii { get; set; }

    public Produkt? Produkt { get; set; }
    public Kategoria? Kategoria { get; set; }
}
