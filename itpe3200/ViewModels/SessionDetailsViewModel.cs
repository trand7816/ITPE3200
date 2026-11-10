namespace itpe3200.ViewModels;
public class SessionDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string JoinCode { get; set; } = "";
    public int MaxGroups { get; set; }
    public bool RandomAssignment { get; set; }
    public DateTime CreatedAt { get; set; }
}