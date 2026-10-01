namespace itpe3200.Models;

public class CourseSession
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public List<Group> Groups { get; set; } = new();
}