namespace itpe3200.DTOs;

public class StudentDetailsDto : StudentDto
{
    public List<GroupDto> Groups { get; set; } = new();
}
