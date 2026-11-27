namespace itpe3200.Models;

public class CourseSession
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string JoinCode { get; set; } = "";
    public bool RandomAssignment { get; set; }
    public int MaxGroups { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Group> Groups { get; set; } = new();

    //Domain rule (rich entity method as in the 3-EntityFramework lecture)
    //Called once when the session is created (see sessionController.Create)
    //Creates empty groups for a session, so students always have a group to join
    public void AddEmptyGroups(int count, int maxSize)
    {
        if (count < 1 || maxSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "A session needs at least one group, and each group at least one place");
        }

        for (int i = 1; i <= count; i++)
        {
            Groups.Add(new Group {Name = $"Group {i}", MaxSize = maxSize});
        }
    }
}