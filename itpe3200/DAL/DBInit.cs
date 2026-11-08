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
            var itpe = new CourseSession { Name = "ITPE3200", JoinCode = "ABC123" };
            var data = new CourseSession { Name = "DATA1700", JoinCode = "XYZ789" };
            var dape = new CourseSession { Name = "DAPE1400", JoinCode = "DEF456" };
            db.CourseSessions.AddRange(itpe, data, dape);
            db.SaveChanges();


            var gruppe1 = new Group { Name = "Gruppe1", MaxSize = 4, CourseSessionId = itpe.Id };
            var gruppe2 = new Group { Name = "Gruppe2", MaxSize = 3, CourseSessionId = itpe.Id };
            var gruppe3 = new Group { Name = "Gruppe3", MaxSize = 5, CourseSessionId = itpe.Id };
            var gruppe4 = new Group { Name = "Gruppe4", MaxSize = 4, CourseSessionId = data.Id };
            var gruppe5 = new Group { Name = "Gruppe5", MaxSize = 4, CourseSessionId = data.Id };
            var gruppe6 = new Group { Name = "Gruppe6", MaxSize = 5, CourseSessionId = dape.Id };
            db.Groups.AddRange(gruppe1, gruppe2, gruppe3, gruppe4, gruppe5, gruppe6);
            db.SaveChanges();

    
            var elev1 = new Student { Name = "Elev1" };
            var ola = new Student { Name = "Ola Nordmann" };
            var kari = new Student { Name = "Kari Nordmann" };
            var per = new Student { Name = "Per Hansen" };
            var anne = new Student { Name = "Anne Larsen" };
            var jonas = new Student { Name = "Jonas Berg" };
            var sofie = new Student { Name = "Sofie Dahl" };
            var emil = new Student { Name = "Emil Strand" };
            var ingrid = new Student { Name = "Ingrid Lie" };
            var lukas = new Student { Name = "Lukas Moen" };
            db.Students.AddRange(elev1, ola, kari, per, anne, jonas, sofie, emil, ingrid, lukas);
            db.SaveChanges();


            var members = new List<GroupMember>
            {
                // ITPE3200
                new GroupMember { StudentId = elev1.Id, GroupId = gruppe1.Id },
                new GroupMember { StudentId = ola.Id, GroupId = gruppe1.Id },
                new GroupMember { StudentId = kari.Id, GroupId = gruppe2.Id },
                new GroupMember { StudentId = per.Id, GroupId = gruppe2.Id },
                new GroupMember { StudentId = anne.Id, GroupId = gruppe2.Id },
                new GroupMember { StudentId = jonas.Id, GroupId = gruppe3.Id },

                // DATA1700
                new GroupMember { StudentId = elev1.Id, GroupId = gruppe4.Id },
                new GroupMember { StudentId = kari.Id, GroupId = gruppe4.Id },
                new GroupMember { StudentId = sofie.Id, GroupId = gruppe4.Id },
                new GroupMember { StudentId = ola.Id, GroupId = gruppe5.Id },
                new GroupMember { StudentId = emil.Id, GroupId = gruppe5.Id },

                // DAPE1400
                new GroupMember { StudentId = per.Id, GroupId = gruppe6.Id },
                new GroupMember { StudentId = ingrid.Id, GroupId = gruppe6.Id },
            };
            db.GroupMembers.AddRange(members);
            db.SaveChanges();
        }
    }
}
