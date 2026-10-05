using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace itpe3200.Models;

public class Group
{
    public int Id { get; set; }
    [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{0,20}", ErrorMessage ="The group name must be numbers or letters between 0 and 20 characters.")]
    [Display(Name = "Group Name")]
    public string Name { get; set; } = string.Empty;
    [Range(0, int.MaxValue, ErrorMessage = "Groups must have a positive value")]
    public int MaxSize { get; set; }
    public int? CourseSessionId { get; set; }
    public CourseSession? CourseSession { get; set; }
    
    public List<Student> Members { get; set; } = new();
}