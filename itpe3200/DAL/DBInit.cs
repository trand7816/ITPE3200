using itpe3200.Models;

namespace itpe3200.DAL;

public static class DbInit
{
    public static void Seed(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // only add test data when the database is empty
        if (!db.CourseSessions.Any())
        {
            // Two course sessions
            var session = new CourseSession
            {
                Name = "ITPE3200",
                JoinCode = "ABC123"
            };
            var otherSession = new CourseSession
            {
                Name = "DATA1700",
                JoinCode = "XYZ789"
            };
            db.CourseSessions.AddRange(session, otherSession);
            db.SaveChanges();

            // one group in each session
            var group = new Group
            {
                Name = "Gruppe1",
                MaxSize = 4,
                CourseSessionId = session.Id
            };
            var otherGroup = new Group
            {
                Name = "Gruppe2",
                MaxSize = 4,
                CourseSessionId = otherSession.Id
            };
            db.Groups.AddRange(group, otherGroup);
            db.SaveChanges();

            // one student saved first so it gets an id
            var student = new Student
            {
                Name = "Elev1"
            };
            db.Students.Add(student);
            db.SaveChanges();

            // Elev1 is in one group in each session through the GroupMember join table
            db.GroupMembers.AddRange(
                new GroupMember { StudentId = student.Id, GroupId = group.Id },
                new GroupMember { StudentId = student.Id, GroupId = otherGroup.Id });
            db.SaveChanges();
        }
    }
}
