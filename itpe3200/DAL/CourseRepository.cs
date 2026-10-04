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

            // Groups and students belong to a session (required foreign key),
            // so EF Core deletes them together with the session (cascade delete).
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

    public async Task<Group?> GetGroupById(int id)
    {
        try
        {
            return await _db.Groups
                .Include(g => g.CourseSession)
                .Include(g => g.Members)
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
            // Load the members too. Student.GroupId is an optional foreign key,
            // and EF Core only sets it to null for students it has loaded.
            // Without Include, SQLite rejects the delete with a foreign key error
            // whenever the group still has members.
            var group = await _db.Groups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == id);

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

    public async Task<Student?> GetStudentById(int id)
    {
        try
        {
            return await _db.Students.FindAsync(id);
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] GetStudentById failed for StudentId {StudentId:0000}, error message: {e}", id, e.Message);
            return null;
        }
    }

    public async Task<IEnumerable<Student>?> GetUnassignedStudents(int sessionId)
    {
        try
        {
            return await _db.Students
                .Where(s => s.CourseSessionId == sessionId && s.GroupId == null)
                .ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] GetUnassignedStudents failed for SessionId {SessionId:0000}, error message: {e}", sessionId, e.Message);
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

    public async Task<bool> AssignStudentToGroup(int studentId, int? groupId)
    {
        try
        {
            var student = await _db.Students.FindAsync(studentId);
            if (student == null)
            {
                _logger.LogError("[CourseRepository] Student not found for StudentId {StudentId:0000}", studentId);
                return false;
            }

            student.GroupId = groupId;
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Assigning StudentId {StudentId:0000} to GroupId {GroupId} failed, error message: {e}", studentId, groupId, e.Message);
            return false;
        }
    }
}