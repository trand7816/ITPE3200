namespace itpe3200.Models;

public class CourseSession
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public string JoinCode { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<Group> Groups { get; set; } = new();
}