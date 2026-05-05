namespace WebApp.Domain;

public class Student
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public ICollection<StudentResult> Results { get; set; } = new List<StudentResult>();
}
