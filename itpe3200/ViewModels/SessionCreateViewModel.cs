using System.ComponentModel.DataAnnotations;

namespace itpe3200.ViewModels;

public class SessionCreateViewModel
{
    [Required(ErrorMessage = "Session name is required")]
    [StringLength(100, ErrorMessage = "Session name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    //Groups set to 10 as default value
    [Range(1, 20, ErrorMessage = "Number of groups must be between 1 and 20")]
    [Display(Name = "Number of groups")]
    public int MaxGroups { get; set; } = 10;

    //Used only when groups are created: Group size default value set to 4.
    //Each group sets its own max size, so it can be changed later.
    [Range(1, 50, ErrorMessage = "Studetns per group must be between 1 and 50")]
    [Display(Name = "Students per group")]
    public int GroupSize { get; set; } = 4;

    public bool RandomAssignment { get; set; }
}
