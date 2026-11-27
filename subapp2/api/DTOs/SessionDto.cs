using System.ComponentModel.DataAnnotations;

namespace itpe3200.DTOs;

public class SessionDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Session name is required")]
    [StringLength(100, ErrorMessage = "Session name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    // Is set by the server when the session is created
    public string JoinCode { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "Number of groups must be between 1 and 20")]
    public int MaxGroups { get; set; } = 10;

    // only used when the session is created, to set the size of the empty groups
    [Range(1, 50, ErrorMessage = "Students per group must be between 1 and 50")]
    public int GroupSize { get; set; } = 4;

    public bool RandomAssignment { get; set; }
}
