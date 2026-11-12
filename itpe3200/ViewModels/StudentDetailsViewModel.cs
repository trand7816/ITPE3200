namespace itpe3200.ViewModels;

// Contains the student information and group memberships shown on the details page.
public class StudentDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public IReadOnlyList<StudentGroupViewModel> Groups { get; set; }
        = Array.Empty<StudentGroupViewModel>();
}

// Represents a group membership together with the course session it belongs to.
public class StudentGroupViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string SessionName { get; set; } = "";
}