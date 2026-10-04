using itpe3200.Models;

namespace itpe3200.DAL;

public static class DbInit
{
    public static void Seed(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!db.CourseSessions.Any())
        {
            var session = new CourseSession
            {
                Name = "ITPE3200",
                JoinCode = "ABC123"
            };
            db.CourseSessions.Add(session);
            db.SaveChanges();

            var group = new Group
            {
                Name = "Gruppe1",
                MaxSize = 4,
                CourseSessionId = session.Id
            };
            db.Groups.Add(group);
            db.SaveChanges();

            db.Students.Add(new Student
            {
                Name = "Elev1",
                CourseSessionId = session.Id,
                GroupId = group.Id
            });
            db.SaveChanges();
        }
    }
}