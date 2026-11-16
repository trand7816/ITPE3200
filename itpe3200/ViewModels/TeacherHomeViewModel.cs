using itpe3200.Models;
namespace itpe3200.ViewModels;

// Dynamic data shown on the teacher dashboard
public class TeacherHomeViewModel
{
    public string Greeting { get; set; } = string.Empty; 
    public string TeacherName { get; set; } = string.Empty;
    public int SessionCount {get; set; }
    public int GroupCount {get; set; }
    public int StudentCount {get; set; }

    // All sessions, shown as cards on the teacher dashboard
    public IEnumerable<CourseSession> Sessions {get; set; } = new List<CourseSession>();
}
