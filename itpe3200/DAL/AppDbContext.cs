using itpe3200.Models;
using Microsoft.EntityFrameworkCore;

namespace itpe3200.DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<CourseSession> CourseSessions { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Student> Students { get; set; }
}