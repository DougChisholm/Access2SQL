using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolTests.Web.Domain;

public class StudentResult
{
    public int ResultId { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int TestId { get; set; }

    [Range(0, 100, ErrorMessage = "Mark must be between 0 and 100.")]
    public int? Mark { get; set; }

    [ForeignKey(nameof(StudentId))]
    public Student? Student { get; set; }

    [ForeignKey(nameof(TestId))]
    public Test? Test { get; set; }
}
