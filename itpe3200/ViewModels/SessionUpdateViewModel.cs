using System.ComponentModel.DataAnnotations;
namespace itpe3200.ViewModels;
public class SessionUpdateViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [Range(0, int.MaxValue)]
    public int MaxGroups { get; set; }

    public bool RandomAssignment { get; set; }
}