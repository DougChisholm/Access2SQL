using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

[Table("Kategorie")]
public class Kategoria
{
    [Key]
    [Column("id_kategorii")]
    public int IdKategorii { get; set; }

    [Column("nazwa")]
    [Required]
    public string Nazwa { get; set; } = string.Empty;

    public ICollection<ProduktKategoria> ProduktKategorie { get; set; } = new List<ProduktKategoria>();
}
