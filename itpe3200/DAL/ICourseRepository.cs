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
    Task<IEnumerable<Student>?> GetAllStudents();
    Task<Student?> GetStudentById(int id);
    Task<bool> AddStudent(Student student);
    Task<bool> UpdateStudent(Student student);
    Task<bool> DeleteStudent(int id);

    // ---------------- Group members ----------------
    // student a member of the group. adds a GroupMember row
    Task<bool> AssignStudentToGroup(int studentId, int groupId);
    Task<bool> RemoveStudentFromGroup(int studentId, int groupId);
    Task<bool?> IsStudentInSession(int studentId, int sessionId);
}