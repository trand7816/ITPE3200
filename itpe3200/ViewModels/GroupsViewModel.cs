using itpe3200.Models;

namespace itpe3200.ViewModels;
public class GroupsViewModel
{
    public IEnumerable<Group> Groups { get; set; } = Enumerable.Empty<Group>();

    public string? Search { get; set; } 

    public int? SessionId { get; set; }

    public IEnumerable<CourseSessionViewModel> CourseSessions { get; set; }
        = Enumerable.Empty<CourseSessionViewModel>();
}