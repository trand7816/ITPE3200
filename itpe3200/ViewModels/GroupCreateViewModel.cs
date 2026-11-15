using System.ComponentModel.DataAnnotations;

namespace itpe3200.ViewModels;


public class GroupCreateViewModel
{
    [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \'*_-]{1,20}", ErrorMessage ="The group name must be numbers, letters or symbols ('*_-) between 1 to 20 characters. The field may also be empty")]
    [Display(Name = "Group Name")]
    public string? Name { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Maximum group size must be at least 1.")]
    [Display(Name = "Group Size")]
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