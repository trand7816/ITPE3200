namespace itpe3200.ViewModels;

public class StudentDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    // The groups this student is a member of, through the GroupMember join table
    public List<StudentGroupViewModel> Groups { get; set; } = new();
}

// One group membership shown on the details page
public class StudentGroupViewModel
{
    public string GroupName { get; set; } = "";
    public string SessionName { get; set; } = "";
    public DateTime JoinedAt { get; set; }
}