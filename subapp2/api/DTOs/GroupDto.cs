using System.ComponentModel.DataAnnotations;

namespace itpe3200.DTOs;

public class GroupDto
{
    public int Id { get; set; }

    [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \'*-]{1,20}", ErrorMessage = "The group name must be numbers or letters between 1 to 20 characters.")]
    public string? Name { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Maximum group size must be at least 1.")]
    public int MaxSize { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Course session is required")]
    public int CourseSessionId { get; set; }

    // Only sent from the server so the client can show the session name
    public string? CourseSessionName { get; set; }
    public int MemberCount { get; set; }
}
