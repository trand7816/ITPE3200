using itpe3200.Models;

namespace itpe3200.ViewModels;

//Data for the group details page
//For teachers only: shows all students - replaces ViewBag.Students
public class GroupDetailsViewModel
{
    public Group Group { get; set; } = new();

    public IEnumerable<Student> Students { get; set; } = new List<Student>();

}