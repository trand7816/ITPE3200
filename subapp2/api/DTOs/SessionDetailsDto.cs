namespace itpe3200.DTOs;

public class SessionDetailsDto : SessionDto
{
    public List<GroupDetailsDto> Groups { get; set; } = new();
}
