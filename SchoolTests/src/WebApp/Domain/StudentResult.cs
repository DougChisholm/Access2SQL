namespace WebApp.Domain;

public class StudentResult
{
    public int ResultId { get; set; }
    public int StudentId { get; set; }
    public int TestId { get; set; }
    public int? Mark { get; set; }

    public Student Student { get; set; } = null!;
    public Test Test { get; set; } = null!;
}
