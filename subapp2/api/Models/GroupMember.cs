namespace itpe3200.Models;

// jin table between Student and Group
public class GroupMember
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int GroupId { get; set; }
    public Group? Group { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
