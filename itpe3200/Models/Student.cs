namespace itpe3200.Models;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    // student can be in several groups but one group per session
    public List<GroupMember> Groups { get; set; } = new();
}
