using itpe3200.Models;

namespace itpe3200.ViewModels;
public class GroupsViewModel
{
    public IEnumerable<Group> Groups { get; set; } = Enumerable.Empty<Group>();
}