using System.ComponentModel.DataAnnotations;
using itpe3200.Models;

namespace itpe3200.ViewModels;

public class JoinViewModel
{
    [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{1,20}", ErrorMessage ="The join code must be numbers or letters between 1 to 20 characters")]
    [Required(ErrorMessage = "Join code is required")]
    public string JoinCode { get; set; } = string.Empty;

    public int? CourseSessionId {get; set;}

    public string? CourseSessionName {get; set;}

    public List<Group>? Groups {get; set;}
}