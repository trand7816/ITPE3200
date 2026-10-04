using Microsoft.EntityFrameworkCore;
using itpe3200.Models;

namespace itpe3200.DAL;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<CourseRepository> _logger;

    public CourseRepository(AppDbContext db, ILogger<CourseRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    // Sessions

    public async Task<IEnumerable<CourseSession>> GetAllSessions()
    {
        return await _db.CourseSessions.ToListAsync();
    }

    public async Task<CourseSession?> GetSessionById(int id)
    {
        return await _db.CourseSessions.Include(s => s.Groups)
                                        .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<CourseSession?> GetSessionByJoinCode(string joinCode)
    {
        return await _db.CourseSessions.FirstOrDefaultAsync(s => s.JoinCode == joinCode);
    }

    public async Task<bool> CreateSession(CourseSession session)
    {
        _db.CourseSessions.Add(session);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateSession(CourseSession session)
    {
        _db.CourseSessions.Update(session);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSession(int id)
    {
        var session = await _db.CourseSessions.FindAsync(id);
        if (session == null)
        {
            return false;
        }

        _db.CourseSessions.Remove(session);
        await _db.SaveChangesAsync();
        return true;
    }

    // Groups

    public async Task<Group?> GetGroupById(int id)
    {
        return await _db.Groups.Include(g => g.Members)
                                .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<bool> CreateGroup(Group group)
    {
        _db.Groups.Add(group);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateGroup(Group group)
    {
        _db.Groups.Update(group);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteGroup(int id)
    {
        var group = await _db.Groups.FindAsync(id);
        if (group == null)
        {
            return false;
        }

        _db.Groups.Remove(group);
        await _db.SaveChangesAsync();
        return true;
    }

    // Students

    public async Task<Student?> GetStudentById(int id)
    {
        return await _db.Students.FindAsync(id);
    }

    public async Task<IEnumerable<Student>> GetUnassignedStudents(int sessionId)
    {
        return await _db.Students.Where(s => s.GroupId == null).ToListAsync();
    }

    public async Task<bool> AddStudent(Student student)
    {
        _db.Students.Add(student);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignStudentToGroup(int studentId, int? groupId)
    {
        var student = await _db.Students.FindAsync(studentId);
        if (student == null)
        {
            return false;
        }

        student.GroupId = groupId;
        await _db.SaveChangesAsync();
        return true;
    }
}