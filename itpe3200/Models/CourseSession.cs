namespace itpe3200.Models;

public class CourseSession
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string JoinCode { get; set; } = "";
    public bool RandomAssignment { get; set; }
    public int MaxGroups { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Group> Groups { get; set; } = new();
    public List<Student> Students { get; set; } = new();
}