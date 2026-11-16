using System.ComponentModel.DataAnnotations;

namespace itpe3200.ViewModels;

public class StudentCreateViewModel
{
    [Required(ErrorMessage = "Student name is required")]
    [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,50}", ErrorMessage = "The name must be letters between 2 and 50 characters")]
    [Display(Name = "Student name")]
    public string Name { get; set; } = string.Empty;
}