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

    public Task<IEnumerable<CourseSession>> GetAllSessions()
        => throw new NotImplementedException();

    public Task<CourseSession?> GetSessionById(int id)
        => throw new NotImplementedException();

    public Task<CourseSession?> GetSessionByJoinCode(string joinCode)
        => throw new NotImplementedException();

    public Task<bool> CreateSession(CourseSession session)
        => throw new NotImplementedException();

    public Task<bool> UpdateSession(CourseSession session)
        => throw new NotImplementedException();

    public Task<bool> DeleteSession(int id)
        => throw new NotImplementedException();

    // Groups

    public Task<Group?> GetGroupById(int id)
        => throw new NotImplementedException();

    public Task<bool> CreateGroup(Group group)
        => throw new NotImplementedException();

    public Task<bool> UpdateGroup(Group group)
        => throw new NotImplementedException();

    public Task<bool> DeleteGroup(int id)
        => throw new NotImplementedException();

    // Students

    public Task<Student?> GetStudentById(int id)
        => throw new NotImplementedException();

    public Task<IEnumerable<Student>> GetUnassignedStudents(int sessionId)
        => throw new NotImplementedException();

    public Task<bool> AddStudent(Student student)
        => throw new NotImplementedException();

    public Task<bool> AssignStudentToGroup(int studentId, int? groupId)
        => throw new NotImplementedException();
}