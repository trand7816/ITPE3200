using Microsoft.AspNetCore.Identity;
using itpe3200.DAL;
using Microsoft.EntityFrameworkCore;

namespace itpe3200;

public static class IdentitySeed
{
    public static async Task SeedAsync(WebApplication app)
    {
        //Create scope to access services such as db and Identity
        using var scope = app.Services.CreateScope();

        //Create a RoleManager
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //Manage a UserManager
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = { "Teacher", "Student" };
        foreach (var role in roles)
        {
            //If the role does not exists, create role
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Could not create role {role}");
                }
            }
        }
        if (app.Environment.IsDevelopment())
        {
            var teacherEmail = app.Configuration["DemoTeacherEmail"];

            //Ensures teacher email is not empty
            if (!string.IsNullOrWhiteSpace(teacherEmail))
            {
                //Tries to find teacher email in db
                var teacher = await userManager.FindByEmailAsync(teacherEmail);

                // Create the demo teacher account if it doesn't exist yet.
                // The teacher is created here instead of through Register, since Register always makes students.
                var teacherPassword = app.Configuration["DemoTeacherPassword"];
                if (teacher == null && !string.IsNullOrWhiteSpace(teacherPassword))
                {
                    teacher = new IdentityUser { UserName = teacherEmail, Email = teacherEmail };
                    var createResult = await userManager.CreateAsync(teacher, teacherPassword);
                    if (!createResult.Succeeded)
                    {
                        throw new InvalidOperationException("Could not create the demo teacher account.");
                    }
                }
                if (teacher != null)
                {
                    //The user is assigned a Teacher role if the user does not already have it
                    if (!await userManager.IsInRoleAsync(teacher, "Teacher"))
                    {
                        var result = await userManager.AddToRoleAsync(teacher, "Teacher");

                        if (!result.Succeeded)
                        {
                            throw new InvalidOperationException("Could not assign Teacher role.");
                        }
                    }
                    
                    // Cleans up teacher accounts that were created through Register that also got a student role / profile.
                    if (await userManager.IsInRoleAsync(teacher, "Student"))
                    {
                        var removeResult = await userManager.RemoveFromRoleAsync(teacher, "Student");
                        if (!removeResult.Succeeded)
                        {
                            throw new InvalidOperationException("Could not remove the Student role from the teacher.");
                        }
                    }

                    // Delete the teacher's student profile, if one was created by Register
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var teacherProfile = await db.Students.FirstOrDefaultAsync(s => s.IdentityUserId == teacher.Id);
                    if (teacherProfile != null)
                    {
                        db.Students.Remove(teacherProfile);
                        await db.SaveChangesAsync();
                    }
                }
            }
        }
    }
}