namespace itpe3200.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int MaxSize { get; set; }
    public string JoinCode { get; set; } = "";
    public int CourseSessionId { get; set; }
    public CourseSession? CourseSession { get; set; }
    public List<Student> Students { get; set; } = new();
}