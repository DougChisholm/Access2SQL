using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

[Table("imiona_damskie")]
public class ImieKobiece
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("imie")]
    public string Imie { get; set; } = string.Empty;
}
