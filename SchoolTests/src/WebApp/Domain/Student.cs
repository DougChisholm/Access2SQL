using System.ComponentModel.DataAnnotations;

namespace SchoolTests.Web.Domain;

public class Student
{
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Student name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    public ICollection<StudentResult> StudentResults { get; set; } = new List<StudentResult>();
}
