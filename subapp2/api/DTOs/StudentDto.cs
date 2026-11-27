using System.ComponentModel.DataAnnotations;

namespace itpe3200.DTOs;

public class StudentDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Student name is required")]
    [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,50}", ErrorMessage = "The name must be letters between 2 and 50 characters")]
    public string Name { get; set; } = string.Empty;
}
