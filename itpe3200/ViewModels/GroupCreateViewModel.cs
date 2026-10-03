namespace itpe3200.ViewModel;

public class GroupCreateViewModel
{
    public string Name { get; set; } = "";
    public int MaxSize { get; set; }
    public int CourseSessionId { get; set; }
    public List<CourseSessionViewModel> CourseSession { get; set; } = new();
}

public class CourseSessionViewModel
{
    public int Id {get; set;}
    public string Name {get; set;} = "";
}