namespace itpe3200.Models;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

 // Null until the student picks a group or is randomly assigned
    public int? GroupId { get; set; }
    public Group? Group { get; set; }

    public int CourseSessionId { get; set; }
    public CourseSession? CourseSession { get; set; }
}