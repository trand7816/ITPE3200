namespace itpe3200.ViewModels;

//Data for the student homepage: greeting and student groups
public class StudentHomeViewModel
{
    public string Greeting { get; set; } = "";

    public string StudentName { get; set; } = "";
    public List<StudentGroupViewModel> Groups { get; set; } = new();

}