using System.ComponentModel.DataAnnotations;

namespace SchoolTests.Web.Domain;

/// <summary>
/// Maps to the Access README table which stores application metadata.
/// </summary>
public class AppInfo
{
    [Key]
    [StringLength(100)]
    public string Item { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Comment { get; set; }
}
