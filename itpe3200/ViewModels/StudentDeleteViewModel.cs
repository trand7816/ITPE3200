namespace itpe3200.ViewModels;

// Contains the student information shown before confirming deletion.
public class StudentDeleteViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public IReadOnlyList<StudentGroupViewModel> Groups { get; set; }
        = Array.Empty<StudentGroupViewModel>();
}
