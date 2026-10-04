using itpe3200.Models;

namespace itpe3200.DAL;

public interface ICourseRepository
{
    // ---------------- Sessions ----------------
    Task<IEnumerable<CourseSession>?> GetAllSessions();
    Task<CourseSession?> GetSessionById(int id);
    Task<CourseSession?> GetSessionByJoinCode(string joinCode);
    Task<bool> CreateSession(CourseSession session);
    Task<bool> UpdateSession(CourseSession session);
    Task<bool> DeleteSession(int id);

    // ---------------- Groups ----------------
    Task<IEnumerable<Group>?> GetAllGroups();
    Task<Group?> GetGroupById(int id);
    Task<bool> CreateGroup(Group group);
    Task<bool> UpdateGroup(Group group);
    Task<bool> DeleteGroup(int id);

    // ---------------- Students ----------------
    Task<Student?> GetStudentById(int id);
    Task<IEnumerable<Student>?> GetUnassignedStudents(int sessionId);
    Task<bool> AddStudent(Student student);
    Task<bool> AssignStudentToGroup(int studentId, int? groupId);
}