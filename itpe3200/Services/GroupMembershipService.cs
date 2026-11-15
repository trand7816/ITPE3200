using itpe3200.DAL;
using itpe3200.Models;

namespace itpe3200.Services;

public class GroupMembershipService
{
    private readonly ICourseRepository _repo;
    private readonly ILogger<GroupMembershipService> _logger;

    public GroupMembershipService(ICourseRepository repo, ILogger<GroupMembershipService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<bool> CanJoinGroup(int studentId, int groupId)
    {
        var group = await _repo.GetGroupById(groupId);

        if(group == null)
        {
            _logger.LogWarning("Group {GroupId} was not found", groupId);
            return false;
        }

        if(group.Members.Count >= group.MaxSize)
        {
            _logger.LogInformation("Group {GroupId} is full", groupId);
            return false;
        }

        //Checks if student is already in a group within the course session
        var isAlreadyMember = await _repo.IsStudentInSession(studentId, group.CourseSessionId);
        //Returns false if student is already in a group or database read fails/returns null (fail safe)
        if(isAlreadyMember != false)
        {
             _logger.LogInformation("Student {StudentId} cannot join Group {GroupdId}. Membership check returned {Result}",studentId, groupId, isAlreadyMember);

            return false;
        }
        
        return true;
    }

/**
    public async Task<bool> JoinGroup(int studentId, int groupId)
    {
        
    }
    **/
}

