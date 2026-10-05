using System.ComponentModel.DataAnnotations;

namespace itpe3200.ViewModels;

public class JoinViewModel
{
    [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]", ErrorMessage ="The name must be letters")]
    [Required(ErrorMessage = "Student name is required")]
    public string Name {get; set; } = string.Empty;

    [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{1,20}", ErrorMessage ="The join code must be numbers or letters between 1 to 20 characters")]
    [Required(ErrorMessage = "Join code is required")]
    public string JoinCode { get; set; } = string.Empty;
}