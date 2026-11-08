using Microsoft.EntityFrameworkCore;
using itpe3200.Models;

namespace itpe3200.DAL;

// Data access layer for sessions, groups and students.
// Every method catches database exceptions, logs them and returns
// null/false instead of crashing. The controller checks the return value
// and decides what the user should see.
public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<CourseRepository> _logger;

    public CourseRepository(AppDbContext db, ILogger<CourseRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    // ---------------- Sessions ----------------

    public async Task<IEnumerable<CourseSession>?> GetAllSessions()
    {
        try
        {
            return await _db.CourseSessions.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] CourseSessions ToListAsync() failed in GetAllSessions(), error message: {e}", e.Message);
            return null;
        }
    }

    public async Task<CourseSession?> GetSessionById(int id)
    {
        try
        {
            return await _db.CourseSessions
                .Include(s => s.Groups)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] GetSessionById failed for SessionId {SessionId:0000}, error message: {e}", id, e.Message);
            return null;
        }
    }

    public async Task<CourseSession?> GetSessionByJoinCode(string joinCode)
    {
        try
        {
            return await _db.CourseSessions.FirstOrDefaultAsync(s => s.JoinCode == joinCode);
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] GetSessionByJoinCode failed for JoinCode {JoinCode}, error message: {e}", joinCode, e.Message);
            return null;
        }
    }

    public async Task<bool> CreateSession(CourseSession session)
    {
        try
        {
            _db.CourseSessions.Add(session);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Session creation failed for session {@session}, error message: {e}", session, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateSession(CourseSession session)
    {
        try
        {
            _db.CourseSessions.Update(session);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Session update failed for SessionId {SessionId:0000}, error message: {e}", session.Id, e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteSession(int id)
    {
        try
        {
            var session = await _db.CourseSessions.FindAsync(id);
            if (session == null)
            {
                _logger.LogError("[CourseRepository] Session not found for SessionId {SessionId:0000}", id);
                return false;
            }

            // Groups belong to a session (required foreign key), so EF Core deletes
            // them and their members, together with the session (cascade delete).
            _db.CourseSessions.Remove(session);
            await _db.SaveChangesAsync();
            return true;        
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Session deletion failed for SessionId {SessionId:0000}, error message: {e}", id, e.Message);
            return false;
        }
    }

    // ---------------- Groups ----------------

    // Used by Group/Table and Group/Grid. Includes the session (to show its name)
    // and the members (to show how many students each group has).
    public async Task<IEnumerable<Group>?> GetAllGroups()
    {
        try
        {
            return await _db.Groups
                .Include(g => g.CourseSession)
                .Include(g => g.Members)
                .ToListAsync();
        }
        catch(Exception e)
        {
            _logger.LogError("[CourseRepository] Groups ToListAsync() failed in GetAllGroups(), error message: {e}", e.Message);
            return null;

        }
    }

    // Used by Group/Details and Group/Delete.
    // ThenInclude loads the student for each member so the view can show the names
    public async Task<Group?> GetGroupById(int id)
    {
        try
        {
            return await _db.Groups
                .Include(g => g.CourseSession)
                .Include(g => g.Members)
                    .ThenInclude(m => m.Student)
                .FirstOrDefaultAsync(g => g.Id == id);
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] GetGroupById failed for GroupId {GroupId:0000}, error message: {e}", id, e.Message);
            return null;
        }
    }

    public async Task<bool> CreateGroup(Group group)
    {
        try
        {
            _db.Groups.Add(group);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Group creation failed for group {@group}, error message: {e}", group, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateGroup(Group group)
    {
        try
        {
            _db.Groups.Update(group);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Group update failed for GroupId {GroupId:0000}, error message: {e}", group.Id, e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteGroup(int id)
    {
        try
        {
            // GroupMember.GroupId is required so EF Core deletes the groups
            // members together with the group (cascade delete).
            // The students themselves are not deleted.
            var group = await _db.Groups.FindAsync(id);

            if (group == null)
            {
                _logger.LogError("[CourseRepository] Group not found for GroupId {GroupId:0000}", id);
                return false;
            }

            _db.Groups.Remove(group);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Group deletion failed for GroupId {GroupId:0000}, error message: {e}", id, e.Message);
            return false;
        }
    }

    // ---------------- Students ----------------

    // Includes the groups so a list of students can show which groups each student is in
    public async Task<IEnumerable<Student>?> GetAllStudents()
    {
        try
        {
            return await _db.Students
                .Include(s => s.Groups)
                    .ThenInclude(m => m.Group)
                .ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Students ToListAsync() failed in GetAllStudents(), error message: {e}", e.Message);
            return null;
        }
    }

    // Includes the groups and their sessions so a details page can show

    public async Task<Student?> GetStudentById(int id)
    {
        try
        {
            return await _db.Students
                .Include(s => s.Groups)
                    .ThenInclude(m => m.Group)
                        .ThenInclude(g => g!.CourseSession)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] GetStudentById failed for StudentId {StudentId:0000}, error message: {e}", id, e.Message);
            return null;
        }
    }

    public async Task<bool> AddStudent(Student student)
    {
        try
        {
            _db.Students.Add(student);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Student creation failed for student {@student}, error message: {e}", student, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateStudent(Student student)
    {
        try
        {
            _db.Students.Update(student);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Student update failed for StudentId {StudentId:0000}, error message: {e}", student.Id, e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteStudent(int id)
    {
        try
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
            {
                _logger.LogError("[CourseRepository] Student not found for StudentId {StudentId:0000}", id);
                return false;
            }

            // GroupMember.StudentId is required, so EF Core deletes the students
 
            _db.Students.Remove(student);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Student deletion failed for StudentId {StudentId:0000}, error message: {e}", id, e.Message);
            return false;
        }
    }

    // ---------------- Group members ----------------

    // Adds a row in the join table so the student becomes a member of the group
    public async Task<bool> AssignStudentToGroup(int studentId, int groupId)
    {
        try
        {
            _db.GroupMembers.Add(new GroupMember
            {
                StudentId = studentId,
                GroupId = groupId
            });
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Adding StudentId {StudentId:0000} to GroupId {GroupId:0000} failed, error message: {e}", studentId, groupId, e.Message);
            return false;
        }
    }

    // Removes the row in the join table. The student and the group are not deleted.
    public async Task<bool> RemoveStudentFromGroup(int studentId, int groupId)
    {
        try
        {
            var member = await _db.GroupMembers
                .FirstOrDefaultAsync(m => m.StudentId == studentId && m.GroupId == groupId);
            if (member == null)
            {
                _logger.LogError("[CourseRepository] StudentId {StudentId:0000} is not a member of GroupId {GroupId:0000}", studentId, groupId);
                return false;
            }

            _db.GroupMembers.Remove(member);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Removing StudentId {StudentId:0000} from GroupId {GroupId:0000} failed, error message: {e}", studentId, groupId, e.Message);
            return false;
        }
    }

    // Checks if the student already is in a group in this session. Used for the rule "only one group per session".
    // Returns null if the database query fails

    public async Task<bool?> IsStudentInSession(int studentId, int sessionId)
    {
        try
        {
            return await _db.GroupMembers
                .AnyAsync(m => m.StudentId == studentId && m.Group!.CourseSessionId == sessionId);
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] IsStudentInSession failed for StudentId {StudentId:0000} and SessionId {SessionId:0000}, error message: {e}", studentId, sessionId, e.Message);
            return null;
        }
    }
}