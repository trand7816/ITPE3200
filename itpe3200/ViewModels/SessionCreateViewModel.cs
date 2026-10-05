using System.ComponentModel.DataAnnotations;

namespace itpe3200.ViewModels;

public class SessionCreateViewModel
{
    [Required(ErrorMessage = "Session name is required")]
    [StringLength(100, ErrorMessage = "Session name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Maximum groups requires a valid number")]
    public int MaxGroups { get; set; }

    public bool RandomAssignment { get; set; }
}
