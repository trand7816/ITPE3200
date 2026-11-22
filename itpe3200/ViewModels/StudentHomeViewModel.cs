namespace itpe3200.ViewModels;

using itpe3200.Models;

//Data for the student homepage: greeting and student groups
public class StudentHomeViewModel

{
    public string Greeting { get; set; } = "";

    public string StudentName { get; set; } = "";
    public List<GroupMember> Groups { get; set; } = new();

}