namespace itpe3200.ViewModels;

// One row in the list of students
public class StudentListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int GroupCount { get; set; }
}