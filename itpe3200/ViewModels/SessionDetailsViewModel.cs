namespace itpe3200.ViewModels;
public class SessionDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string JoinCode { get; set; } = "";
    public int MaxGroups { get; set; }
    public bool RandomAssignment { get; set; }
    public DateTime CreatedAt { get; set; }

    public IReadOnlyList<SessionGroupViewModel> Groups { get; set; }
     = Array.Empty<SessionGroupViewModel>();
}

public class SessionGroupViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int MemberCount { get; set; }
    public int MaxSize { get; set; }
    public IReadOnlyList<string> Members { get; set; }
        = Array.Empty<string>();
}