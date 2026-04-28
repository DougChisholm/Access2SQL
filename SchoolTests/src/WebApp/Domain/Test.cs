using System.ComponentModel.DataAnnotations;

namespace SchoolTests.Web.Domain;

public class Test
{
    public int TestId { get; set; }

    [Required(ErrorMessage = "Test description is required.")]
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
    [Display(Name = "Test Description")]
    public string TestDescription { get; set; } = string.Empty;

    public ICollection<StudentResult> StudentResults { get; set; } = new List<StudentResult>();
}
