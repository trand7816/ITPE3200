using Microsoft.EntityFrameworkCore;
using itpe3200.Models;

namespace itpe3200.DAL;

public class GroupDbContext : DbContext
{
    public GroupDbContext(DbContextOptions<GroupDbContext> options) : base(options)
    {
    }

    public DbSet<CourseSession> CourseSessions { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Student> Students { get; set; }
}