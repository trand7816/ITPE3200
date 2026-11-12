using System.ComponentModel.DataAnnotations;

namespace itpe3200.ViewModels;


// Carries the validated student data submitted when a student's name is updated.

public class StudentUpdateViewModel
{
   public int Id { get; set; }

    [Required(ErrorMessage = "Student name is required.")]
    [StringLength(20, ErrorMessage = "Student name cannot exceed 20 characters.")]
    public string Name { get; set; } = "";
}