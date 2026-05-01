namespace WebApp.Domain;

public class Test
{
    public int TestId { get; set; }
    public string TestDescription { get; set; } = string.Empty;
    public ICollection<StudentResult> Results { get; set; } = new List<StudentResult>();
}
