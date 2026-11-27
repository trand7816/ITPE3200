namespace itpe3200.DTOs;

public class GroupDetailsDto : GroupDto
{
    public List<MemberDto> Members { get; set; } = new();
}
