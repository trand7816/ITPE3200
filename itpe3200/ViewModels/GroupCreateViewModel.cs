using System.ComponentModel.DataAnnotations;

namespace itpe3200.ViewModels;


public class GroupCreateViewModel
{
    public string Name { get; set; } = string.Empty;
    [Range(0, int.MaxValue, ErrorMessage = "Group size requires a valid number")]
    public int MaxSize { get; set; }
    [Required(ErrorMessage = "Course session is required")]
    public int? CourseSessionId { get; set; }
    
    public List<CourseSessionViewModel> CourseSessions { get; set; } = new();
}

public class CourseSessionViewModel
{
    public int Id {get; set;}
    public string Name {get; set;} = "";
}