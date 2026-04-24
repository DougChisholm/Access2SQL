using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

[Table("nazwiska_damskie")]
public class NazwiskoKobiece
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nazwisko")]
    public string Nazwisko { get; set; } = string.Empty;
}
